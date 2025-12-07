using ScottPlot;

namespace OnePlusOne
{
    internal class AlgorithmOnePlusOne
    {
        private Random _rand = new Random();

        public double Fitness(double x)
        {
            return Math.Sin(x / 10.0) * Math.Sin(x / 200.0);
        }

        public double RandomDouble(double min, double max)
        {
            return min + _rand.NextDouble() * (max - min);
        }

        public void RunOnePlusOne(double minX, double maxX, double rozrzutPoczatkowy, double wspPrzyrostu, int l_Iteracji, double? startMin = null, double? startMax = null, int[] itersSteps = null)
        {
            double sMin = startMin ?? minX;
            double sMax = startMax ?? maxX;
            // 1. x = początkowa wartość losowo wybrana z przedziału zakres_zmienności,
            double x = RandomDouble(sMin, sMax);
            //2. y = wartość funkcji przystosowania(tutaj sin(x/10.0).*sin(x./200)),
            double y = Fitness(x);

            double rozrzut = rozrzutPoczatkowy;

            List<double> ListX = new List<double>();
            List<double> ListY = new List<double>();
            Console.WriteLine("Iter\t x\t\t y\t\t rozrzut");

            if (itersSteps == null || itersSteps.Contains(0))
            {
                Console.WriteLine($"0\t {x:F6}\t {y:F6}\t");
                // Console.WriteLine($"0\t {y:F6}\t {rozrzut:F6}");
                // Console.WriteLine($"0\t {x:F6}\t {y:F6}\t {rozrzut:F6}");
            }

            ListX.Add(x);
            ListY.Add(y);
            //3. wykonaj pętlę l_iteracji razy
            for (int i = 1; i <= l_Iteracji; i++)
            {
                //3.1. xpot = x + zmienna losowa z przedziału[-rozrzut; +rozrzut]
                double delta = RandomDouble(-rozrzut, rozrzut);
                double xpot = x + delta;

                //3.2. jeśli wartość parametru xpot przekroczyła dozwolony przedział(zakres_zmienności) należy go skorygować wybraną przez siebie metodą.
                if (xpot < minX) xpot = minX;
                if (xpot > maxX) xpot = maxX;
                //3.3. ypot = funkcja przystosowania dla xpot,
                double ypot = Fitness(xpot);

                //3.4.jeśli uzyskano nie gorszy wynik(ypot >= y)

                if (ypot >= y)
                {
                    //3.4.1. zastąp stare wartości nowymi(x = xpot; y = ypot)
                    x = xpot;
                    y = ypot;
                    //3.4.2. zwiększ rozrzut o wsp_przyrostu(rozrzut *= wsp_przyrostu)
                    rozrzut *= wspPrzyrostu;
                }
                //3.5. jeśli uzyskano gorszy wynik(ypot<y)
                else
                {
                    //3.5.1.zmniejsz rozrzut o wsp_przyrostu(rozrzut /= wsp_przyrostu)
                    rozrzut /= wspPrzyrostu;
                }
                

                ListX.Add(x);
                ListY.Add(y);

                if (itersSteps == null || itersSteps.Contains(i))
                {
                    Console.WriteLine($"{i}\t {x:F6}\t {y:F6}\t");
                    // Console.WriteLine($"{i}\t {y:F6}\t {rozrzut:F6}");
                    // Console.WriteLine($"{i}\t {x:F6}\t {y:F6}\t {rozrzut:F6}");
                }

            }
            VisualizeResultsPlot(minX, maxX, ListX, ListY);

        }
        
        public void VisualizeResultsPlot(double minX, double maxX, List<double> ListX, List<double> ListY)
        {
            var plot = new ScottPlot.Plot();

            int pointCount = 1000;
            double[] Listxpoint = new double[pointCount];
            double[] Listypoint = new double[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                double x = minX + i * (maxX - minX) / (pointCount - 1);
                Listxpoint[i] = x;
                Listypoint[i] = Fitness(x);
            }

            var scatter = plot.Add.Scatter(Listxpoint, Listypoint);
            scatter.LegendText = "Funkcja Przystosowania f(x)";
            scatter.Color = ScottPlot.Colors.Blue;
            scatter.MarkerSize = 0;

            var points = plot.Add.Scatter(ListX.ToArray(), ListY.ToArray());
            points.LegendText = "Wyniki Algorytmu 1+1";
            points.Color = ScottPlot.Colors.Red;
            points.MarkerSize = 5;

            plot.Title("Wykorzystanie Algorytmu 1+1 do optymalizacji funkcji przystosowania");
            plot.ShowLegend();
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            
            string fullPath = Path.GetFullPath(Path.Combine(projectDir, @"../../.."));

            string savePath = Path.Combine(fullPath, "One_plus_one_algorythm.png");
            plot.SavePng(savePath, 800, 600);

        }
            static void Main(string[] args)
        {
            var oneplusone = new AlgorithmOnePlusOne();

            double mainMinX = 0;
            double mainMaxX = 100;
            double wspPrzyrostu = 1.1;

            int[] steps = new int[] { 0, 5, 10, 15 };
            // Console.WriteLine("---Zadanie 1---");
            // oneplusone.RunOnePlusOne(mainMinX, mainMaxX, rozrzutPoczatkowy: 10, wspPrzyrostu, l_Iteracji: 100, itersSteps: steps);
            //

            int[] steps20 = Enumerable.Range(0, 21).ToArray();
            // Console.WriteLine("---Zadanie 2---");
            // oneplusone.RunOnePlusOne(mainMinX, mainMaxX, rozrzutPoczatkowy: 10, wspPrzyrostu, l_Iteracji: 100, itersSteps: steps20);

            Console.WriteLine("---Zadanie 3---");
            oneplusone.RunOnePlusOne(minX: mainMinX, maxX: mainMaxX, rozrzutPoczatkowy: 5, 
            wspPrzyrostu: wspPrzyrostu, l_Iteracji: 100, startMin: 15, startMax: 35, itersSteps: steps20);
        }
    }
}
