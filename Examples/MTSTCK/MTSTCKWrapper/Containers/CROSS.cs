
namespace MTSTCKWrapper.Containers
{
    public class CROSS : DataSeries<double, bool>
    {
        public CROSS(ICustomData<double, double> series, string refName, CrossDirection direction) 
            : base(direction == CrossDirection.Down? series.Name + "CROSS" + refName: refName + "CROSS" + series.Name)
        {
            Series = series;
            Direction = direction;
        }

        internal override bool GetCustomValue(double value, int index)
        {
            if (Series.Count < index + 1) 
            {
                return false;
            }                

            if (Direction == CrossDirection.Down) 
            {
                return Series.GetValueByIndex(index) > value && Series.GetValueByIndex(index + 1) < value;
            }
            else
            {
                return Series.GetValueByIndex(index) < value && Series.GetValueByIndex(index + 1) > value;
            }             
        }

        private readonly ICustomData<double, double> Series;

        private readonly CrossDirection Direction;
    }
}
