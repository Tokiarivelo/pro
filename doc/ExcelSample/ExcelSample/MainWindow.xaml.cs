using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var excelApplication = new NetOffice.ExcelApi.Application())
            {

                try
                {
                    // Création du fichier de travail.
                    var workBook = excelApplication.Workbooks.Add();
                    // Positionnement sur le premier onglet.
                    var workSheet = (NetOffice.ExcelApi.Worksheet)workBook.Worksheets[1];

                    // Ajout des données dans les différentes cellules.
                    workSheet.Cells[1, 2].Value = "Langues (en millions de locuteurs) :";
                    workSheet.Cells[2, 1].Value = "Mandarin";
                    workSheet.Cells[2, 2].Value = "860";
                    workSheet.Cells[3, 1].Value = "Espagnol";
                    workSheet.Cells[3, 2].Value = "469";
                    workSheet.Cells[4, 1].Value = "Anglais";
                    workSheet.Cells[4, 2].Value = "362";
                    workSheet.Cells[5, 1].Value = "Arabe";
                    workSheet.Cells[5, 2].Value = "276";
                    workSheet.Cells[6, 1].Value = "Bengali";
                    workSheet.Cells[6, 2].Value = "270";
                    workSheet.Cells[7, 1].Value = "Hindi";
                    workSheet.Cells[7, 2].Value = "269";
                    workSheet.Cells[8, 1].Value = "Portugais";
                    workSheet.Cells[8, 2].Value = "222";
                    workSheet.Cells[9, 1].Value = "Russe";
                    workSheet.Cells[9, 2].Value = "150";
                    workSheet.Cells[10, 1].Value = "Japonais";
                    workSheet.Cells[10, 2].Value = "125";
                    workSheet.Cells[11, 1].Value = "Lahnda/Pendjabi";
                    workSheet.Cells[11, 2].Value = "112";

                    // Ajout d'un diagramme et positionnement.
                    var chart = ((NetOffice.ExcelApi.ChartObjects)workSheet.ChartObjects()).Add(150, 200, 500, 400);
                    // Définition du type de diagramme.
                    chart.Chart.ChartType = NetOffice.ExcelApi.Enums.XlChartType.xlPie;
                    // Association du diagramme aux données de la grille.
                    chart.Chart.SetSourceData(workSheet.Range("A1:B11"));

                    // Enregistrement du fichier.
                    workBook.SaveAs(@"C:\Users\toky\Desktop\newDep\test.xlsx");
                }
                finally
                {
                    // Sortie de l'application Excel.
                    excelApplication.Quit();
                }
            }
        }
    }
}
