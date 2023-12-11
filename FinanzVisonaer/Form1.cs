using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace FinanzVisonaer
{
    public partial class Form1 : Form
    {
        private bool IsNumeric(string text)
        {
            double num;
            return double.TryParse(text, out num);
        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            richTextBox1.KeyPress += richTextBox1_KeyPress; 
            richTextBox1.Validating += richTextBox1_Validating;
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
                    textBox1.Text += richTextBox2.Text + " " + richTextBox3.Text + "\r\n";
                    label9.Text = richTextBox3.Text;
                    if (double.TryParse(label10.Text, NumberStyles.Any, culture, out double value1) &&
                   double.TryParse(richTextBox3.Text, NumberStyles.Any, culture, out double value2))
                    {
                        double difference = value1 - value2;

                        // Das Ergebnis in das Label schreiben
                        label10.Text = difference.ToString("0.0", culture);
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
        /// <summary>
        /// Es fehlt noch  eine Art graph 
        /// und ich möchte das noch in einer
        /// Datenbank speichern 
        /// 
        /// </summary>

    }
}
