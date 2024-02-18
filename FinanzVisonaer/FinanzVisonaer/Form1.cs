using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using Excel = Microsoft.Office.Interop.Excel;


namespace FinanzVisonaer
{
    public partial class Form1 : Form
    {
        // Liste für die Werte in richTextBox3
        private List<double> richTextBox3Values = new List<double>();

        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();

        // Methode zur Überprüfung, ob ein Text eine Zahl ist
        private bool IsNumeric(string text)
        {
            double num;
            return double.TryParse(text, out num); //überprüfen ob es eine Zahl ist
        }

        


        public Form1()
        {
            InitializeComponent();
            
        }
        // Initialisierung beim Laden der Form
        private void Form1_Load(object sender, EventArgs e)
        {
            // Event-Handler für richTextBox1
            richTextBox1.KeyPress += richTextBox1_KeyPress;
            richTextBox1.Validating += richTextBox1_Validating;


            comboBox1.Text = "--Select--";
            label1.Visible = false; richTextBox1.Visible = false;
            groupBox1.Visible = false; label12.Visible = false;
            checkBox1.Visible = false;

            InitializeChart();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            label7.Text =  richTextBox1.Text;
            label9.Text = "0";
            label10.Text =  richTextBox1.Text;
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Erlaubte Zeichen: Zahlen (0-9), Punkt (.) und Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Erlaube nur einen Punkt
            if ((e.KeyChar == '.') && ((sender as RichTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void richTextBox1_Validating(object sender, CancelEventArgs e)
        {
            // Überprüfen, ob der Text eine gültige Zahl ist
            string text = richTextBox1.Text;
            if (!string.IsNullOrEmpty(text))
            {
                if (!IsNumeric(text))
                {
                    MessageBox.Show("Ungültige Zahl!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Clear(); //  clear den text kann aber noch geändert  werden wfalls
            //   Checkt die Box (Visible true/false)
            if(checkBox1.Checked)
            {
                groupBox3.Visible = true;
                groupBox4.Visible = true;
            }
            else
            { 
                groupBox3.Visible = false; 
                groupBox4.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            SqlConnection connection1 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=TrueData Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=TrueData Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=True");
            try
            {
                connection1.Open();
                // Ersetzen Sie Column1 durch Ihren tatsächlichen Spaltennamen.

                String query = "Insert Into SpeichernWerte (Name,Abzug) Values (@Name, @Abzug)";
                SqlCommand cmd = new SqlCommand(query, connection1);
                cmd.Parameters.AddWithValue("@Name",richTextBox1.Text);
                cmd.Parameters.AddWithValue("@Abzug", richTextBox2.Text);
                cmd.ExecuteNonQuery();

                connection1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Werte: " + ex.Message);
            }
            */
            
            CultureInfo culture = CultureInfo.InvariantCulture;
            try
            {
                if (checkBox1.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(richTextBox2.Text) && !string.IsNullOrWhiteSpace(richTextBox3.Text))
                    {
                        undoStack.Push(textBox1.Text); // Speichere den aktuellen Zustand in undoStack

                        textBox1.Text += richTextBox2.Text + " " + richTextBox3.Text + "€" + "\r\n";

                        if (double.TryParse(label10.Text, NumberStyles.Any, culture, out double value1) &&
                            double.TryParse(richTextBox3.Text, NumberStyles.Any, culture, out double value2))
                        {
                            richTextBox3Values.Add(value2);
                            double difference = value1 - value2;
                            double sum = richTextBox3Values.Sum();

                            label10.Text = difference.ToString("0.0", culture);
                            label9.Text = sum.ToString("0.0", culture);
                            label10.ForeColor = difference < 0 ? System.Drawing.Color.Red : System.Drawing.Color.ForestGreen;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bitte fülle beide Textboxen aus.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    richTextBox2.Text = "";
                    richTextBox3.Text = "";
                    UpdateUI();
                }
                else
                {
                    richTextBox2.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ein Fehler ist aufgetreten: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Abfrage1(bool i)
        {
            groupBox1.Visible = i;
            label1.Visible = i; richTextBox1.Visible = i;
            groupBox1.Visible = i; label12.Visible = i;
            checkBox1.Visible = i;
 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool i = false;
            groupBox5.Visible = false;
            if (comboBox1.Text == "Statistik")
            {
                Abfrage1(i);
                groupBox5.Visible = true;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                checkBox1.Checked = false;
            }
            else if(comboBox1.Text == "Budget-Rechner") 
            {
                i = true;
                Abfrage1(i);
                
            }
            else if(comboBox1.Text == "--Select--")
            {
                Abfrage1(false);
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                checkBox1.Checked = false;
            }
            else
            {
                Abfrage1(i);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {/*
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=TrueData Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=TrueData Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\noahn\source\repos\FinanzVisonaer\FinanzVisonaer\Database1.mdf;Integrated Security=True");
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select * from SpeichernWerte");
                SqlDataReader reader = command.ExecuteReader();
                string s = "";

                while(reader.Read())
                {
                    s = s + reader.GetString(1) + ";" + reader.GetString(2);
                }
                MessageBox.Show(s);

            }
            catch
            {
                throw;
            }
            connection.Close();
             */
             if(!string.IsNullOrEmpty(textBox1.Text)) // wenn textbox leer ist
             {
                 // Hole die Werte aus TextBox1
                 string textValue = textBox1.Text;

                 // Trenne die Werte nach Leerzeichen
                 string[] values = textValue.Split(' ');


                 SaveFileDialog saveFileDialog = new SaveFileDialog();
                 saveFileDialog.Filter = "Excel Files|*.xlsx";
                 saveFileDialog.Title = "Daten in Excel speichern";

                 if (saveFileDialog.ShowDialog() == DialogResult.OK)
                  {
                     // Speichere die Werte in Excel
                     SaveToExcel(values, saveFileDialog.FileName);
                  }
             }
             else
             {
                 MessageBox.Show("Keine Ausgaben vorhanden");
             }
            
        }
        // Methode zum Speichern in Excel
        private void SaveToExcel(string[] values, string filePath)
        {
            try
            {
                // Erstelle eine Excel-Anwendung
                Excel.Application excelApp = new Excel.Application();

                // Erstelle eine neue Arbeitsmappe
                Excel.Workbook workbook = excelApp.Workbooks.Add();

                // Erstelle ein Arbeitsblatt
                Excel.Worksheet worksheet = workbook.Sheets[1];

                // Setze die Überschriften für die Spalten
                worksheet.Cells[1, 1] = "Name";
                worksheet.Cells[1, 2] = "Kosten (€)";

                // Schreibe die Werte in die Excel-Tabelle
                for (int i = 0; i < values.Length; i += 2) // Schreibe zwei Werte in einer Iteration
                {
                    worksheet.Cells[(i / 2) + 2, 1] = values[i]; // Name in Spalte 1
                    worksheet.Cells[(i / 2) + 2, 2] = values[i + 1]; // Kosten in Spalte 2
                }

                // Speichere die Arbeitsmappe
                workbook.SaveAs(filePath);

                // Schließe die Excel-Anwendung
                excelApp.Quit();

                MessageBox.Show("Daten wurden in Excel gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Daten in Excel: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                string previousState = undoStack.Pop();
                redoStack.Push(textBox1.Text);

                double label9Value = 0;

                if (double.TryParse(label9.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out label9Value))
                {
                    double sum = richTextBox3Values.Sum() - label9Value;
                    label9.Text = sum.ToString("0.0", CultureInfo.InvariantCulture);
                }

                textBox1.Text = previousState;
                UpdateUI();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                string nextState = redoStack.Pop();
                undoStack.Push(textBox1.Text);

                double label9Value = 0;

                if (double.TryParse(label9.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out label9Value))
                {
                    double sum = richTextBox3Values.Sum() + label9Value;
                    label9.Text = sum.ToString("0.0", CultureInfo.InvariantCulture);
                }

                textBox1.Text = nextState;
                UpdateUI();
            }
        }
        private void InitializeChart()
        {
            // Initialisiere das Diagramm mit den erforderlichen Einstellungen
            chart1.Series.Clear();
            chart1.Series.Add("Ausgaben");
            chart1.Series["Ausgaben"].ChartType = SeriesChartType.Pie;
        }
        private void UpdateChart()
        {
            // Lösche bestehende Daten und füge neue Daten hinzu
            chart1.Series["Ausgaben"].Points.Clear();

            foreach (double value in richTextBox3Values)
            {
                chart1.Series["Ausgaben"].Points.Add(value);
            }
        }

        private void UpdateStatistics()
        {
            // Berechne Statistiken (zum Beispiel Gesamtsumme, Durchschnitt)
            double sum = richTextBox3Values.Sum();
            double average = richTextBox3Values.Count > 0 ? richTextBox3Values.Average() : 0;

            // Aktualisiere Label für Statistiken
            label26.Text = $"{sum:C}";
            label23.Text = $"{average:C}";
        }

        private void UpdateUI()
        {
            UpdateChart();
            UpdateStatistics();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Erlaubte Zeichen: Zahlen (0-9), Punkt (.) und Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Erlaube nur einen Punkt
            if ((e.KeyChar == '.') && ((sender as RichTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void richTextBox3_Validating(object sender, CancelEventArgs e)
        {
            // Überprüfen, ob der Text eine gültige Zahl ist
            string text = richTextBox3.Text;
            if (!string.IsNullOrEmpty(text))
            {
                if (!IsNumeric(text))
                {
                    MessageBox.Show("Ungültige Zahl!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// probleme noch beim Speichern 
        /// </summary>

    }
}
