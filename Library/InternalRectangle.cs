namespace Library;

internal abstract class InternalRectangle
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
}

//Прямоугольники, которые можно пересекать с очень большим штрафом (в случае блок-схем символы)
internal class ForbiddenRectangle : InternalRectangle
{
    
}

internal class LineRectangle : InternalRectangle
{
    internal OrientationValues Orientation { get; set; }
    internal enum OrientationValues
    {
        Horizontal, //Участок линии горизонтальный
        Vertical, //Участок линии вертикальный
        Turn //Участок линии с переходом с вертикаль на горизонтали и наоборот
    }
}

internal class FreeRectangle : InternalRectangle
{
    public List<ForbiddenRectangle> HorizontalForbiddenRectangles { get; } = [];
    public List<ForbiddenRectangle> VerticalForbiddenRectangles { get; } = [];
}

internal class Mapper(int margin)
//Отступ от точек поворота и создание маленьких квадратиков через который возможно прохождение только с большим штрафом
//отступ от линии дя формирования прямоугольного канала
{
    public List<InternalRectangle> Map(RectangleDTO rectangleDTO)
    {
        return
        [
            new ForbiddenRectangle
            {
                X1 = rectangleDTO.X1,
                Y1 = rectangleDTO.Y1,
                X2 = rectangleDTO.X2,
                Y2 = rectangleDTO.Y2,
            }
        ];       
    }

    public List<InternalRectangle> Map(LineDTO lineDTO)
    {
        var res = new List<InternalRectangle>();

        int x = lineDTO.XStart;
        int y = lineDTO.YStart;
        var orientation = lineDTO.OrientationStart;

        int x1Turn = 0;
        int x2Turn = 0;
        int y1Turn = 0;
        int y2Turn = 0;

        for (int i = 0; i < lineDTO.Segments.Count; i++)
        {
            int currentMargin = 0;
            if (i != lineDTO.Segments.Count - 1)
            {
                currentMargin = margin;
            }

            int signedLength = lineDTO.Segments[i].SignedLength;

            if (i != 0)
            {
                switch (orientation)
                {
                    case LineDTO.OrientationValues.Horizontal:
                        if (signedLength > margin)
                        {
                            if (y1Turn == y)
                            {
                                y1Turn -= margin;
                                signedLength -= x2Turn - x;
                                x = x2Turn;
                            }
                            else
                            {
                                y2Turn += margin;
                                signedLength -= x2Turn - x;
                                x = x2Turn;
                            }

                            res.Add(new LineRectangle
                            {
                                X1 = x1Turn,
                                Y1 = y1Turn,
                                X2 = x2Turn,
                                Y2 = y2Turn,
                                Orientation = LineRectangle.OrientationValues.Turn,
                            });
                        }
                        else if (signedLength > 0)
                        {
                            if (y1Turn == y)
                            {
                                y1Turn -= margin;
                                x2Turn = x + signedLength;
                                signedLength -= x2Turn - x;
                                x = x2Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                y2Turn += margin;
                                x2Turn = x + signedLength;
                                signedLength -= x2Turn - x;
                                x = x2Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (signedLength >= -margin)
                        {
                            if (y1Turn == y)
                            {
                                y1Turn -= margin;
                                x1Turn = x + signedLength;
                                signedLength -= x1Turn - x;
                                x = x1Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                y2Turn += margin;
                                x1Turn = x + signedLength;
                                signedLength -= x1Turn - x;
                                x = x1Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (y1Turn == y)
                            {
                                y1Turn -= margin;
                                signedLength -= x1Turn - x;
                                x = x1Turn;
                            }
                            else
                            {
                                y2Turn += margin;
                                signedLength -= x1Turn - x;
                                x = x1Turn;
                            }

                            res.Add(new LineRectangle
                            {
                                X1 = x1Turn,
                                Y1 = y1Turn,
                                X2 = x2Turn,
                                Y2 = y2Turn,
                                Orientation = LineRectangle.OrientationValues.Turn,
                            });
                        }
                        break;
                    case LineDTO.OrientationValues.Vertical:
                        if (signedLength > margin)
                        {
                            if (x1Turn == x)
                            {
                                x1Turn -= margin;
                                signedLength -= y2Turn - y;
                                y = y2Turn;
                            }
                            else
                            {
                                x2Turn += margin;
                                signedLength -= y2Turn - y;
                                y = y2Turn;
                            }

                            res.Add(new LineRectangle
                            {
                                X1 = x1Turn,
                                Y1 = y1Turn,
                                X2 = x2Turn,
                                Y2 = y2Turn,
                                Orientation = LineRectangle.OrientationValues.Turn,
                            });
                        }
                        else if (signedLength > 0)
                        {
                            if (x1Turn == x)
                            {
                                x1Turn -= margin;
                                y2Turn = y + signedLength;
                                signedLength -= y2Turn - y;
                                y = y2Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                x2Turn += margin;
                                y1Turn = y + signedLength;
                                signedLength -= y1Turn - y;
                                y = y1Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (signedLength >= -margin)
                        {
                            if (x1Turn == x)
                            {
                                x1Turn -= margin;
                                y1Turn = y + signedLength;
                                signedLength -= y1Turn - y;
                                y = y1Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                x2Turn += margin;
                                y1Turn = y + signedLength;
                                signedLength -= y1Turn - y;
                                y = y1Turn;

                                if (i == lineDTO.Segments.Count - 1)
                                {
                                    res.Add(new LineRectangle
                                    {
                                        X1 = x1Turn,
                                        Y1 = y1Turn,
                                        X2 = x2Turn,
                                        Y2 = y2Turn,
                                        Orientation = LineRectangle.OrientationValues.Turn,
                                    });
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (x1Turn == x)
                            {
                                x1Turn -= margin;
                                signedLength -= y1Turn - y;
                                y = y1Turn;
                            }
                            else
                            {
                                x2Turn += margin;
                                signedLength -= y1Turn - y;
                                y = y1Turn;
                            }

                            res.Add(new LineRectangle
                            {
                                X1 = x1Turn,
                                Y1 = y1Turn,
                                X2 = x2Turn,
                                Y2 = y2Turn,
                                Orientation = LineRectangle.OrientationValues.Turn,
                            });
                        }
                        break;
                }

                switch (orientation)
                {
                    case LineDTO.OrientationValues.Horizontal:
                        if (signedLength > margin)
                        {
                            int x1 = x;
                            int y1 = y - margin;
                            int x2 = x + signedLength - margin;
                            int y2 = y + margin;

                            x1Turn = x2;
                            y1Turn = y - margin;
                            x2Turn = x + signedLength;
                            y2Turn = y + margin;

                            x = x + signedLength;
                            res.Add(new LineRectangle
                            {
                                Orientation = LineRectangle.OrientationValues.Horizontal,
                                X1 = x1,
                                Y1 = y1,
                                X2 = x2,
                                Y2 = y2,
                            });
                        }
                        else if (signedLength >= 0)
                        {
                            x1Turn = x;
                            x2Turn = x + signedLength;
                            y1Turn = y - margin;
                            y2Turn = y + margin;

                            //Сегмент настолько мал что войдёт внутрь прямоугольника вокруг точки поворота
                        }
                        else if (signedLength >= -margin)
                        {
                            x1Turn = x + signedLength;
                            x2Turn = x;
                            y1Turn = y - margin;
                            y2Turn = y + margin;

                            //Сегмент настолько мал что войдёт внутрь прямоугольника вокруг точки поворота
                        }
                        else
                        {
                            int x1 = x + signedLength + margin;
                            int y1 = y - margin;
                            int x2 = x;
                            int y2 = y + margin;

                            x1Turn = x + signedLength;
                            x2Turn = x1;
                            y1Turn = y - margin;
                            y2Turn = y + margin;

                            x = x + signedLength;

                            res.Add(new LineRectangle
                            {
                                Orientation = LineRectangle.OrientationValues.Horizontal,
                                X1 = x1,
                                Y1 = y1,
                                X2 = x2,
                                Y2 = y2,
                            });
                        }

                        break;
                    case LineDTO.OrientationValues.Vertical:
                        if (signedLength > margin)
                        {
                            int x1 = x - margin;
                            int y1 = y;
                            int x2 = x + margin;
                            int y2 = y + signedLength - margin;

                            x1Turn = x1;
                            x2Turn = x2;
                            y1Turn = y2;
                            y2Turn = y + signedLength;

                            y = y + signedLength;
                            res.Add(new LineRectangle
                            {
                                Orientation = LineRectangle.OrientationValues.Vertical,
                                X1 = x1,
                                Y1 = y1,
                                X2 = x2,
                                Y2 = y2,
                            });
                        }
                        else if (signedLength >= 0)
                        {
                            x1Turn = x - margin;
                            x2Turn = x + margin;
                            y1Turn = y;
                            y2Turn = y + signedLength;
                        }
                        else if (signedLength >= -margin)
                        {
                            //Сегмент настолько мал что войдёт внутрь прямоугольника вокруг точки поворота

                            x1Turn = x - margin;
                            x2Turn = x + margin;
                            y1Turn = y + signedLength;
                            y2Turn = y;
                        }
                        else
                        {
                            int x1 = x - margin;
                            int y1 = y + signedLength + margin;
                            int x2 = x + margin;
                            int y2 = y;

                            x1Turn = x - margin;
                            x2Turn = x + margin;
                            y1Turn = y + signedLength;
                            y2Turn = y1;

                            y = y + signedLength;

                            res.Add(new LineRectangle
                            {
                                Orientation = LineRectangle.OrientationValues.Vertical,
                                X1 = x1,
                                Y1 = y1,
                                X2 = x2,
                                Y2 = y2,
                            });
                        }

                        break;
                }

                if (orientation == LineDTO.OrientationValues.Horizontal)
                {
                    orientation = LineDTO.OrientationValues.Vertical;
                }
                else
                {
                    orientation = LineDTO.OrientationValues.Horizontal;
                }
            }
        }
    }
}
