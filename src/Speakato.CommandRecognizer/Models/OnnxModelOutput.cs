using Microsoft.ML.Data;

namespace Speakato.CommandRecognizer.Models
{
    internal class OnnxModelOutput
    {
        [ColumnName("dense_3")]
        public float[] PredictedFare { get; set; }
    }
}
