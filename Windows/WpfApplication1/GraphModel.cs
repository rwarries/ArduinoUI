using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1
{
    public class GraphModel
    {
            private MainWindow _mw;
            private const double margin = 10;
            private const double step = 10;
            private const double xmin = margin;
            private const double ymin = margin;
            private double xmax; 
            private double ymax; 

        private GraphModel(){
            throw new NotImplementedException("GraphModel requires a window. No argument constructor cannot be used");
        }

        public GraphModel(MainWindow window){
            _mw = window;
            xmax = _mw.canGraph.Width - margin;
            ymax = _mw.canGraph.Height - margin;
        }


        public void DrawStaticParts()
        {
            double xmin = margin;
            double xmax = _mw.canGraph.Width - margin;
            double ymin = margin;
            double ymax = _mw.canGraph.Height - margin;
            const double step = 10;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(_mw.canGraph.Width, ymax)));
            for (double x = xmin + step;
                x <= _mw.canGraph.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            _mw.canGraph.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, _mw.canGraph.Height)));
            for (double y = step; y <= _mw.canGraph.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            _mw.canGraph.Children.Add(yaxis_path);
        }

        internal void DrawData()
        {
           // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            Random rand = new Random();
            for (int data_set = 0; data_set < 3; data_set++)
            {
                int last_y = rand.Next((int)ymin, (int)ymax);

                PointCollection points = new PointCollection();
                for (double x = xmin; x <= xmax; x += step)
                {
                    last_y = rand.Next(last_y - 10, last_y + 10);
                    if (last_y < ymin) last_y = (int)ymin;
                    if (last_y > ymax) last_y = (int)ymax;
                    points.Add(new Point(x, last_y));
                }

                Polyline polyline = new Polyline();
                polyline.StrokeThickness = 1;
                polyline.Stroke = brushes[data_set];
                polyline.Points = points;

                _mw.canGraph.Children.Add(polyline);
            }
        }
        }
    }
