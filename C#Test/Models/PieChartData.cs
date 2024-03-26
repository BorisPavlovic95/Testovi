namespace C_Test.Models
{
    public class PieChartData
    {
        public string Label { get; set; }
        public double Value { get; set; }

        public PieChartData(string label, double value)
        {
            Label = label;
            Value = value;
        }
    }
}
