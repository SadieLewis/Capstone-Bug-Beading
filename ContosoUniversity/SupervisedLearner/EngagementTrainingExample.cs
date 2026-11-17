using System;

namespace BugBeading.SupervisedLearner
{
    public class EngagementTrainingExample
    {
        //Identifies product represented by this example, used for tracking
        public int ProductId { get; set; }

        //The period used to calculate the input features
        public DateTime ObservationStartUtc { get; set; }

        public DateTime ObservationEndUtc { get; set; }

        //Input features
        public float TotalHovers { get; set; }

        public float TotalClicks { get; set; }

        public float ImageHovers { get; set; }

        public float ImageClicks { get; set; }

        public float TotalHoverDuration { get; set; }

        public float AverageHoverDuration { get; set; }

        public float UniquePages { get; set; }

        //Output label - actual engagement in the following period.
        public bool HighEngagement { get; set; }
    }
}