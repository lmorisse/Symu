#region Licence

// Description: SymuBiz - SymuScenariosAndEvents
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Chart;

#endregion

namespace SymuExamples.ScenariosAndEvents
{
    public static class ChartAppearance
    {
        public static void ApplyChartStyles(ChartControl chart)
        {
            if (chart == null)
            {
                throw new ArgumentNullException(nameof(chart));
            }

            #region Chart Appearance Customization

            chart.Skins = Skins.Metro;
            chart.BorderAppearance.SkinStyle = ChartBorderSkinStyle.None;
            chart.BorderAppearance.FrameThickness = new ChartThickness(-2, -2, 2, 2);
            chart.SmoothingMode = SmoothingMode.AntiAlias;
            chart.ChartArea.PrimaryXAxis.HidePartialLabels = true;
            chart.ElementsSpacing = 5;

            #endregion

            #region Axes Customization

            chart.PrimaryYAxis.RangeType = ChartAxisRangeType.Set;
            chart.PrimaryXAxis.RangeType = ChartAxisRangeType.Set;
            if (chart.Series.Count == 0 || chart.Series[0].Points.Count == 0)
            {
                return;
            }

            var max = chart.Series[0].Points[0].YValues[0];
            for (var i = 0; i < chart.Series.Count; i++)
            {
                for (var j = 0; j < chart.Series[i].Points.Count; j++)
                {
                    max = Math.Max(max, chart.Series[i].Points[j].YValues[0]);
                }
            }

            chart.PrimaryYAxis.Range = new MinMaxInfo(0, max + 1, Math.Round(max / 10));
            var min = chart.Series[0].Points[0].X;
            max = min;
            for (var i = 0; i < chart.Series.Count; i++)
            {
                for (var j = 0; j < chart.Series[i].Points.Count; j++)
                {
                    min = Math.Min(min, chart.Series[i].Points[j].X);
                    max = Math.Max(max, chart.Series[i].Points[j].X);
                }
            }

            chart.PrimaryXAxis.Range = new MinMaxInfo(min - 10, max + 10, Math.Round((max - min) / 10));

            chart.PrimaryXAxis.LabelRotate = true;
            chart.PrimaryXAxis.LabelRotateAngle = 270;

            chart.Series[0].Style.Border.Color = Color.Transparent;

            #endregion
        }
    }
}