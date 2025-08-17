using MySql.Data.MySqlClient;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace library_management_system
{
    public class ReportsViewModel
    {
        public PlotModel BooksBorrowedModel { get; }
        public PlotModel BookCategoriesModel { get; }

        private string connStr = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public ReportsViewModel()
        {
            BooksBorrowedModel = CreateBooksBorrowedModel();
            BookCategoriesModel = CreateBookCategoriesModel();
        }

        private PlotModel CreateBooksBorrowedModel()
        {
            var model = new PlotModel
            {
                Title = "Monthly Book Borrowing Trends",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Transparent,
                PlotMargins = new OxyThickness(60, 20, 20, 60)
            };

            var legend = new Legend
            {
                LegendTitle = "Series",
                LegendTextColor = OxyColors.White,
                LegendPosition = LegendPosition.RightTop
            };
            model.Legends.Add(legend);

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Month",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(40, 255, 255, 255)
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Books Borrowed",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                Minimum = 0,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(40, 255, 255, 255)
            };

            var lineSeries = new LineSeries
            {
                Title = "Books Borrowed",
                Color = OxyColor.FromRgb(70, 130, 180),
                StrokeThickness = 3,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.White
            };

            // Prepare last 6 months labels and initialize values
            var months = new List<string>();
            var values = new List<double>();
            var now = DateTime.Now;

            for (int i = 5; i >= 0; i--)
            {
                var dt = now.AddMonths(-i);
                months.Add(dt.ToString("MMM"));
                values.Add(0);
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // Query borrowing counts grouped by year-month for last 6 months
                    string sql = @"
                        SELECT DATE_FORMAT(borrow_date, '%Y-%m') AS YearMonth, COUNT(*) AS Count
                        FROM transactions
                        WHERE borrow_date >= DATE_FORMAT(DATE_SUB(CURDATE(), INTERVAL 5 MONTH), '%Y-%m-01')
                        GROUP BY YearMonth
                        ORDER BY YearMonth";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string yearMonth = reader.GetString("YearMonth"); // e.g. "2025-03"
                            int count = reader.GetInt32("Count");

                            // Map YearMonth to index in last 6 months
                            for (int i = 0; i < months.Count; i++)
                            {
                                string monthKey = now.AddMonths(i - 5).ToString("yyyy-MM");
                                if (yearMonth == monthKey)
                                {
                                    values[i] = count;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
            }

            // Add axis labels and data points
            for (int i = 0; i < months.Count; i++)
            {
                categoryAxis.Labels.Add(months[i]);
                lineSeries.Points.Add(new DataPoint(i, values[i]));
            }

            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.Series.Add(lineSeries);

            return model;
        }

        private PlotModel CreateBookCategoriesModel()
        {
            var model = new PlotModel
            {
                Title = "Book Categories Distribution",
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White
            };

            var pieSeries = new PieSeries
            {
                StrokeThickness = 2,
                InsideLabelFormat = "{1}: {0:0}",
                TextColor = OxyColors.White
            };

            try
            {
                var genreCounts = new Dictionary<string, int>();

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT genre, COUNT(*) AS Count FROM books GROUP BY genre";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string genre = reader.GetString("genre");
                            int count = reader.GetInt32("Count");

                            genreCounts[genre] = count;
                        }
                    }
                }

                var rnd = new Random();
                foreach (var kvp in genreCounts)
                {
                    pieSeries.Slices.Add(new PieSlice(kvp.Key, kvp.Value)
                    {
                        Fill = OxyColor.FromRgb(
                            (byte)rnd.Next(30, 220),
                            (byte)rnd.Next(30, 220),
                            (byte)rnd.Next(30, 220))
                    });
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
            }

            model.Series.Add(pieSeries);
            return model;
        }
    }
}
