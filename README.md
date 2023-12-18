using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using Excel = Microsoft.Office.Interop.Excel;


namespace FinanzVisonaer
{
    public partial class Form1 : Form
    {
        private List<double> richTextBox3Values = new List<double>();  //Speichern der Werte in einer Liste

        private bool IsNumeric(string text)
        {
            double num;
            return double.TryParse(text, out num); //überprüfen ob es eine Zahl ist
        }
        
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            richTextBox1.KeyPress += richTextBox1_KeyPress; 
            richTextBox1.Validating += richTextBox1_Validating;
          
            comboBox1.Text = "--Select--";
            label1.Visible = false; richTextBox1.Visible = false;
            groupBox1.Visible = false; label12.Visible = false;
            checkBox1.Visible = false;
               

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

        private void label12_Click(object sender, EventArgs e)
        {

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
            CultureInfo culture = CultureInfo.InvariantCulture;
            if (checkBox1.Checked)
            {
                // Überprüfe, ob richTextBox2 und richTextBox3 Text enthalten
                if (!string.IsNullOrWhiteSpace(richTextBox2.Text) && !string.IsNullOrWhiteSpace(richTextBox3.Text))
                {
                    textBox1.Text += richTextBox2.Text + "  " + richTextBox3.Text+ "€" + "\r\n";
                   
                    if (double.TryParse(label10.Text, NumberStyles.Any, culture, out double value1) &&
                   double.TryParse(richTextBox3.Text, NumberStyles.Any, culture, out double value2))
                    {
                        richTextBox3Values.Add(value2);
                        double difference = value1 - value2;
                        double sum = richTextBox3Values.Sum();
                        
                        // Das Ergebnis in das Label schreiben
                        label10.Text = difference.ToString("0.0", culture);
                        label9.Text = sum.ToString("0.0", culture);
                        label10.ForeColor = difference < 0 ? System.Drawing.Color.Red : System.Drawing.Color.ForestGreen;
                    }
                }
                else
                {
                    // Zeige eine Fehlermeldung an, wenn richTextBox2 oder richTextBox3 leer sind
                    MessageBox.Show("Bitte fülle beide Textboxen aus.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                richTextBox2.Text = "";
                richTextBox3.Text = "";
            }
            else { richTextBox2.Visible = false; }      
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
            if (comboBox1.Text == "Ausgaben-Rechner")
            {
                Abfrage1(i);
                groupBox5.Visible = true;

            }
            else if(comboBox1.Text == "Budget-Rechner") 
            {
                i = true;
                Abfrage1(i);
                
            }
            else if(comboBox1.Text == "--Select--")
            {
                Abfrage1(false);
                
            }
            else
            {
                Abfrage1(i);
            }
            
        }
        private void test23()
        {

        }

        private void button2_Click(object sender, EventArgs e)
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

                // Schreibe die Werte in die Excel-Tabelle
                for (int i = 0; i < values.Length; i++)
                {
                    worksheet.Cells[1, i + 1] = values[i];
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
        /// <summary>
        /// Menü um die  Ausgaben auf zu schreiben art tabelle evtl ?
        /// Excel als speicher Element bei tastdruck 
        /// Graphen speichern möglich wäre ein Chart
        /// ss
        /// </summary>

    }
}
