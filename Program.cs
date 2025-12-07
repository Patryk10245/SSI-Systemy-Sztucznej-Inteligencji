using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Text.RegularExpressions;

namespace Kmeans
{
    internal class Kmeans
    {
        public List<double[]> LoadCSVData(string path) //wczytanie zawartości z pliku CSV
        {
            var lines = File.ReadAllLines(path);
            var data = new List<double[]>();

            foreach (var line in lines)
            {
                string[] parts = line.Split(',');

                double x1 = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                double x2 = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);

                data.Add(new double[] { x1, x2 });
            }
            return data;
        }

        public List<double[]> CentroidsAfter4;
        public int[] AssignmentsAfter4;

        public List<double[]> CentroidsAfter10;
        public int[] AssignmentsAfter10;
        public List<double[]> InitializeCentroids(List<double[]> samples, int m)
        {
            //1. Wybierz losowo m różnych próbek i uznaj je jako środki grup(V)
            Random rand = new Random();
            HashSet<int> selectedIndices = new HashSet<int>();
            List<double[]> centroids = new List<double[]>();

            while (centroids.Count < m)
            {
                int index = rand.Next(samples.Count);
                if (!selectedIndices.Contains(index))
                {
                    selectedIndices.Add(index);
                    double[] centroid = (double[])samples[index].Clone();
                    centroids.Add(centroid);
                }
            }
            return centroids;  //centroids to środki grup(V)
        }
        public void RunKMeans(List<double[]> samples, int m, int iters, bool useX1Distance)
        {
            //samples to zbiór M próbek
            //m to liczba grup
            //iters to liczba iteracji

        //1. Inicjalizacja środków grup(V)
        List<double[]> centroids = InitializeCentroids(samples, m);

            int M = samples.Count; // liczba próbek
            int[] us = new int[M]; //us to lista przechowująca indeksy grup dla każdej próbki
            
            //2. Pętla wykonywana zadaną liczbę iteracji(iters)
            for (int i = 0; i < iters; i++)
            {
                //2.1. Pętla po wszystkich M próbkach, s to indeks aktualnej próbki
                for (int s = 0; s < M; s++)
                {
                    double[] sample = samples[s];
                    double bestDistance = double.MaxValue;
                    int bestCentroid = 0;

                    //2.1.1. Wylicz odległości między próbką s a każdym środkiem grupy(V)
                    for (int j = 0; j < centroids.Count; j++)
                    {
                        double distance = useX1Distance
                            ? DistanceX1(sample, centroids[j])
                            : DistanceEuclidean(sample, centroids[j]);
                        if (distance < bestDistance)
                        {  
                            bestDistance = distance;
                            bestCentroid = j;
                        }
                    }
                    //2.1.2. Wyznacz us równy indeksowi najbliższego środka grupy
                    us[s] = bestCentroid;
                }

                int dimension = samples[0].Length; // liczba atrybutów w próbce
                //2.2. Pętla po wszystkich m grupach, j to indeks aktualnej grupy
                for (int j = 0; j < m; j++)
                {
                    List<double[]> Xgr = new List<double[]>(); // zbiór próbek należących do grupy j
                    //2.2.1. Wybierz próbki, należące do tej grupy(zbiór próbek o indeksach s, takich, że us == j), niech zbiór ten nazywa się Xgr
                    for (int s = 0; s < M; s++)
                    {
                        if (us[s] == j) // Xgr
                        {
                            Xgr.Add(samples[s]);
                        }
                    }
                    //2.2.2. Jeśli zbiór Xgr jest pusty, wtedy pomiń wykonanie dalszej części tej pętli.
                    if (Xgr.Count == 0)
                    {
                        continue;
                    }

                    //2.2.3. Pętla po wszystkich atrybutach, i to index poszczególnego atrybutu
                    for (int attr_i = 0; attr_i < dimension; attr_i++)
                    {
                        double sum = 0;

                        foreach (var sample in Xgr)
                        {
                            sum += sample[attr_i];
                        }
                        //2.2.3.1 Wartość i-tego atrybutu grupy j-tej to średnia wartość atrybutu i-tego wszystkich próbek Xgr
                        centroids[j][attr_i] = sum / Xgr.Count;
                    }

                
                if (i == 3 || i == iters - 1)
                {
                    double minX1 = double.MaxValue;
                    double maxX1 = double.MinValue;
                    double minX2 = double.MaxValue;
                    double maxX2 = double.MinValue;

                    foreach (var sample in Xgr)
                    {
                        double x1 = sample[0];
                        double x2 = sample[1];

                        if (x1 < minX1) minX1 = x1;
                        if (x1 > maxX1) maxX1 = x1;

                        if (x2 < minX2) minX2 = x2;
                        if (x2 > maxX2) maxX2 = x2;
                    }

                    int iterNumber = (i == 3) ? 4 : iters;

                    Console.WriteLine($"\n=== Raport po {iterNumber} iteracjach, grupa {j} ===");
                    Console.WriteLine($"Środek: ({centroids[j][0]}, {centroids[j][1]})");
                    Console.WriteLine($"Liczba próbek: {Xgr.Count}");
                    Console.WriteLine($"x1: min = {minX1}, max = {maxX1}");
                    Console.WriteLine($"x2: min = {minX2}, max = {maxX2}");
                }
            }
           
                if (i == 3)
                {
                    CentroidsAfter4 = centroids
                        .Select(c => (double[])c.Clone())
                        .ToList();

                    AssignmentsAfter4 = (int[])us.Clone();
                }
            }
                CentroidsAfter10 = centroids
                    .Select(c => (double[])c.Clone())
                    .ToList();

                AssignmentsAfter10 = (int[])us.Clone();
        }

        public double DistanceEuclidean(double[] s, double[] V) 
        { 
            double sum = 0;
            for (int i = 0; i < s.Length; i++) 
            {
                sum += Math.Pow(s[i] - V[i], 2);
            }
            return Math.Sqrt(sum);

        }

        public double DistanceX1(double[] s, double[] V) 
        {
            return Math.Abs(s[0] - V[0]);
        }

        public void VisualisePlot(List<double[]> samples, List<double[]> centroids, int[] assignments, 
            int m, string fileName, string title)
        {
            ScottPlot.Plot plot = new();

            for (int j = 0; j < m; j++)
            {
                List<double> xList = new List<double>();
                List<double> yList = new List<double>();

                for (int s = 0; s < samples.Count; s++)
                {
                    if (assignments[s] == j)
                    {
                        xList.Add(samples[s][0]);
                        yList.Add(samples[s][1]);
                    }
                }
                if (xList.Count > 0)
                {
                    var scatter = plot.Add.Scatter(xList.ToArray(), yList.ToArray());
                    scatter.LineStyle = ScottPlot.LineStyle.None;
                    scatter.MarkerSize = 6;
                    scatter.LegendText = $"Grupa {j + 1}";
                }
            }
            List<double> centroidX = new List<double>();
            List<double> centroidY = new List<double>();
            foreach (var centroid in centroids)
            {
                centroidX.Add(centroid[0]);
                centroidY.Add(centroid[1]);
            }

            var scatterCentroids = plot.Add.Scatter(centroidX.ToArray(), centroidY.ToArray());
            scatterCentroids.MarkerSize = 10;
            scatterCentroids.LineStyle = ScottPlot.LineStyle.None;
            scatterCentroids.Color = ScottPlot.Colors.Black;
            scatterCentroids.LegendText = "Centroidy";
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            
            string fullPath = Path.GetFullPath(Path.Combine(projectDir, @"../../.."));

            string savePath = Path.Combine(fullPath, fileName);
            plot.Title(title);
            plot.ShowLegend();
            plot.SavePng(savePath, 800, 600);
        }

        static void Main(string[] args)
        {
            var kmeans = new Kmeans();
            var kmeansx1 = new Kmeans();
            var samples = kmeans.LoadCSVData("../../../spiralka.csv");
            Console.WriteLine("\n---------------Euclides---------------\n");
            kmeans.RunKMeans(samples, m: 3, iters: 10, useX1Distance: false);
            
            kmeans.VisualisePlot(samples, kmeans.CentroidsAfter4, kmeans.AssignmentsAfter4, m: 3,
                fileName: "kmeans_euclid_iter4.png",
                title: "K-średnie (x1) - 4 iteracje" );
            
            kmeans.VisualisePlot(samples, kmeans.CentroidsAfter10, kmeans.AssignmentsAfter10, m: 3,
                fileName: "kmeans_euclid_iter10.png",
                title: "K-średnie (x1) - 10 iteracji");
            
            Console.WriteLine("Centroidy po 4 iteracjach:");
            foreach (var centroid in kmeans.CentroidsAfter4)
                Console.WriteLine($"({centroid[0]}, {centroid[1]})");
            
            Console.WriteLine("Centroidy po 10 iteracjach:");
            foreach (var centroid in kmeans.CentroidsAfter10)
                Console.WriteLine($"({centroid[0]}, {centroid[1]})");
            Console.WriteLine("\n---------------X1---------------\n");
            kmeansx1.RunKMeans(samples, m: 4, iters: 10, useX1Distance: true);
            
            kmeansx1.VisualisePlot(samples, kmeansx1.CentroidsAfter4, kmeansx1.AssignmentsAfter4, m: 4,
                fileName: "kmeans_x1_iter4.png",
                title: "K-średnie (x1) - 4 iteracje" );
            
            kmeansx1.VisualisePlot(samples, kmeansx1.CentroidsAfter10, kmeansx1.AssignmentsAfter10, m: 4,
                fileName: "kmeans_x1_iter10.png",
                title: "K-średnie (x1) - 10 iteracji");
            
            Console.WriteLine("Centroidy po 4 iteracjach:");
            foreach (var centroid in kmeansx1.CentroidsAfter4)
                Console.WriteLine($"({centroid[0]}, {centroid[1]})");
            
            Console.WriteLine("Centroidy po 10 iteracjach:");
            foreach (var centroid in kmeansx1.CentroidsAfter10)
                Console.WriteLine($"({centroid[0]}, {centroid[1]})");

        }
    }
}
