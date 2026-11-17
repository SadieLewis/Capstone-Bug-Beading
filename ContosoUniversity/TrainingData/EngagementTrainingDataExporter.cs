
using System.Text.Json;

namespace BugBeading.TrainingData
{
    public class EngagementTrainingDataExporter
    {
        private readonly EngagementTrainingDataBuilder _builder;
        private readonly IWebHostEnvironment _environment;

        public EngagementTrainingDataExporter(
            EngagementTrainingDataBuilder builder,
            IWebHostEnvironment environment)
        {
            _builder = builder;
            _environment = environment;
        }

        public async Task<string> ExportTrainingDataAsync(
            DateTime historyStartUtc,
            DateTime historyEndUtc,
            int periodDays = 7,
            CancellationToken cancellationToken = default)
        {
            //Build the training examples using our existing builder.
            var trainingExamples =
                await _builder.BuildTrainingDataAsync(
                    historyStartUtc,
                    historyEndUtc,
                    periodDays,
                    cancellationToken);

            //Determine where the Node.js training project is located.
            var outputDirectory = Path.Combine(
                _environment.ContentRootPath,
                "SupervisedLearner",
                "node-training"
            );

            //Create the directory if it does not already exist.
            Directory.CreateDirectory(outputDirectory);

            var outputPath = Path.Combine(
                outputDirectory,
                "training-data.json"
            );

            //Use camelCase so the JSON properties are easy to consume in JavaScript.
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            //Serialize the training examples.
            var json = JsonSerializer.Serialize(
                trainingExamples,
                options
            );

            //Write the dataset to the Node.js project.
            await File.WriteAllTextAsync(
                outputPath,
                json,
                cancellationToken
            );

            return outputPath;
        }
    }
}
