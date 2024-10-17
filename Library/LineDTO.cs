namespace Library;

public class LineDTO
{
    public int XStart { get; set; }
    public int YStart { get; set; }

    public List<Segment> Segments { get; set; } = []; //Сегменты поочерёдны то горизонтальные то вертикальные 

    public enum OrientationValues
    {
        Horizontal,
        Vertical
    }

    public OrientationValues OrientationStart { get; set; }

    public class Segment
    {
        private int signedLength;
        public int SignedLength // 0 Запрещён
        {
            get => signedLength;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("");
                }
            }
        } 
    }
}