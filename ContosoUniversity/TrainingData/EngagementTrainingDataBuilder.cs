using BugBeading.Data;
using BugBeading.Models;
using BugBeading.SupervisedLearner;
using Microsoft.EntityFrameworkCore;

namespace BugBeading.TrainingData
{
    public class EngagementTrainingDataBuilder
    {
        private readonly InventoryContext _context;

        public EngagementTrainingDataBuilder(InventoryContext context)
        {
            _context = context;
        }

        public async Task<List<EngagementTrainingExample>>
            BuildTrainingDataAsync(
                DateTime historyStartUtc,
                DateTime historyEndUtc,
                int periodDays = 7,
                CancellationToken cancellationToken = default)
        {
            if (historyStartUtc.Kind != DateTimeKind.Utc ||
                historyEndUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Date range must use UTC timestamps.");
            }

            if (historyEndUtc <= historyStartUtc)
            {
                throw new ArgumentException(
                    "History end must be after the start.");
            }

            if (periodDays <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(periodDays));
            }

            //Retrieve product associated interactions from the requeste historical range.
            var interactions = await _context.UserInteractions
                .AsNoTracking()
                .Where(x =>
                    x.TimestampUtc >= historyStartUtc &&
                    x.TimestampUtc < historyEndUtc &&
                    x.ProductId.HasValue)
                .ToListAsync(cancellationToken);

            //Include existing products, even if they received no interactions during this period.
            var currentProductIds = await _context.Products
                .AsNoTracking()
                .Select(x => x.ID)
                .ToListAsync(cancellationToken);

            //Also retain historical product IDs if the product has since been removed from inventory.
            var productIds = currentProductIds
                .Concat(interactions.Select(x => x.ProductId!.Value))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var trainingExamples =
                new List<EngagementTrainingExample>();

            if (productIds.Count == 0)
                return trainingExamples;

            var windowLength = TimeSpan.FromDays(periodDays);
            var fullWindowLength = TimeSpan.FromDays(periodDays * 2);

            //Each training example uses one observation period followed by one outcome period.
            for (var observationStart = historyStartUtc;
                 observationStart + fullWindowLength <= historyEndUtc;
                 observationStart += fullWindowLength)
            {
                var observationEnd =
                    observationStart + windowLength;

                var outcomeEnd =
                    observationEnd + windowLength;

                var observationRecords = interactions
                    .Where(x =>
                        x.TimestampUtc >= observationStart &&
                        x.TimestampUtc < observationEnd)
                    .ToList();

                var outcomeRecords = interactions
                    .Where(x =>
                        x.TimestampUtc >= observationEnd &&
                        x.TimestampUtc < outcomeEnd)
                    .ToList();

                //Count actual interactions during the future outcome period for each product.
                var futureScores = productIds.ToDictionary(
                    productId => productId,
                    productId => outcomeRecords.Count(x =>
                        x.ProductId == productId &&
                        IsTrackedInteraction(x.InteractionType))
                );

                var orderedScores = futureScores.Values
                    .OrderBy(x => x)
                    .ToList();

                //We need variation in future engagement to create meaningful high/low labels.
                if (orderedScores.Distinct().Count() < 2)
                    continue;
                double medianScore;
                int middle = orderedScores.Count / 2;
                if (orderedScores.Count % 2 == 0)
                {
                    medianScore = (orderedScores[middle - 1] + orderedScores[middle]) / 2.0;
                }
                else
                {
                    medianScore = orderedScores[middle];
                }
                //Build one training example per product.
                foreach (var productId in productIds)
                {
                    var productRecords = observationRecords
                        .Where(x => x.ProductId == productId)
                        .ToList();

                    var hovers = productRecords
                        .Where(x => x.InteractionType == "hover")
                        .ToList();

                    var imageHovers = productRecords
                        .Where(x => x.InteractionType == "image_hover")
                        .ToList();

                    var clicks = productRecords
                        .Where(x => x.InteractionType == "click")
                        .ToList();

                    var imageClicks = productRecords
                        .Where(x => x.InteractionType == "image_click")
                        .ToList();

                    var hoverCount =
                        hovers.Count + imageHovers.Count;

                    var totalHoverDuration = productRecords
                        .Where(x =>
                            x.InteractionType == "hover" ||
                            x.InteractionType == "image_hover")
                        .Sum(x => x.DurationMilliseconds);

                    var averageHoverDuration =
                        hoverCount > 0
                            ? (float)totalHoverDuration / hoverCount
                            : 0f;

                    var uniquePages = productRecords
                        .Select(x => x.Page)
                        .Distinct()
                        .Count();

                    trainingExamples.Add(
                        new EngagementTrainingExample
                        {
                            ProductId = productId,
                            ObservationStartUtc = observationStart,
                            ObservationEndUtc = observationEnd,
                            TotalHovers = hovers.Count,
                            TotalClicks = clicks.Count,
                            ImageHovers = imageHovers.Count,
                            ImageClicks = imageClicks.Count, 
                            TotalHoverDuration = totalHoverDuration, 
                            AverageHoverDuration = averageHoverDuration, 
                            UniquePages = uniquePages,
                            //The label comes from the following observation period, not the inputs.
                            HighEngagement = futureScores[productId] > medianScore
                        }
                    );
                }
            }
            return trainingExamples;
        }

        private static bool IsTrackedInteraction(string type)
        {
            return type == "hover" ||
                   type == "image_hover" ||
                   type == "click" ||
                   type == "image_click";
        }
    }
}