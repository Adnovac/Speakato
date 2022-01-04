using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;
using System;

namespace Speakato.CommandRecognizer.Models
{
    internal class OnnxModelInput
    {
        [ColumnName("dense_input"), OnnxMapType(typeof(float), typeof(Single))]
        [VectorType(96, 1)]
        public float[] Vector { get; set; }
    }
}
