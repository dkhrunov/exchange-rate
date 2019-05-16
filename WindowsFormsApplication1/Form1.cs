using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> valutes = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            chart1.Visible = false;

            string date = DateTime.Now.ToString("dd/MM/yyyy").Replace('.', '/');
            string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date;

            XmlTextReader reader = new XmlTextReader(url);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        if (reader.Name == "Valute")
                        {
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (reader.Name == "ID")
                                    {
                                        string code = reader.Value;
                                        while (reader.Name != "CharCode")
                                        {
                                            reader.Read();
                                        }
                                        reader.Read();

                                        valutes.Add(code, reader.Value);
                                    }
                                }
                            }
                        }

                        break;
                }
            }
            reader.Close();

            foreach (KeyValuePair<string, string> keyValue in valutes)
            {
                comboBox1.Items.Add(keyValue.Value);
            }
           
            // Курс USD и EUR на сегодня
            groupBox1.Text = "Курс доллара и евро на " + date;

            string urlDaily = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date;
            XmlTextReader readerDaily = new XmlTextReader(urlDaily);

            while (readerDaily.Read())
            {
                switch (readerDaily.NodeType)
                {
                    case XmlNodeType.Element:

                        if (readerDaily.Name == "Valute")
                        {
                            if (readerDaily.HasAttributes)
                            {
                                while (readerDaily.MoveToNextAttribute())
                                {
                                    // USD
                                    if (readerDaily.Name == "ID")
                                    {
                                        if (readerDaily.Value == "R01235")
                                        {
                                            while (readerDaily.Name != "Value")
                                            {
                                                readerDaily.Read();
                                            }

                                            readerDaily.Read();
                                            textBox1.Text = readerDaily.Value;
                                        }
                                    }

                                    //EUR
                                    if (readerDaily.Name == "ID")
                                    {
                                        if (readerDaily.Value == "R01239")
                                        {
                                            while (readerDaily.Name != "Value")
                                            {
                                                readerDaily.Read();
                                            }

                                            readerDaily.Read();
                                            textBox2.Text = readerDaily.Value;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                }
            }
            readerDaily.Close();
        }

        private void compareDateToPresent(DateTimePicker date)
        {

            if (DateTime.Compare(date.Value.Date, DateTime.Now.Date) > 0)
            {
                date.Value = DateTime.Now;
                MessageBox.Show("Я не умею предсказывать курс валют!");
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + 
                          dateTimePicker1.Value.ToString("dd/MM/yyyy").Replace('.', '/');

            XmlTextReader reader = new XmlTextReader(url);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "CharCode")
                        {
                            reader.Read();
                            if (reader.Value == comboBox1.Text)
                            {
                                while (reader.Name != "Nominal")
                                {
                                    reader.Read();
                                }
                                reader.Read();
                                label5.Text = "Курс " + reader.Value + " " + comboBox1.Text + ":";
                            }
                        }
                        break;
                }
            }
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || dateTimePicker1.Value.ToString() == "")
            {
                MessageBox.Show("Заполните поля выше!");
            }
            else
            {
                string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" +
                              dateTimePicker1.Value.ToString("dd/MM/yyyy").Replace('.', '/');

                XmlTextReader reader = new XmlTextReader(url);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "CharCode")
                            {
                                reader.Read();
                                if (reader.Value == comboBox1.Text)
                                {
                                    while (reader.Name != "Value")
                                    {
                                        reader.Read();
                                    }
                                    reader.Read();
                                    textBox4.Text = reader.Value + " руб.";
                                }
                            }
                            break;
                    }
                }
                reader.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool btnCliked = false;

            TimeSpan timeSpan = dateTimePicker3.Value.Date - dateTimePicker2.Value.Date;
            if (timeSpan.TotalDays < 6)
            {
                MessageBox.Show("Минимальный период изменения цен на валюту 6 дней!");
            }
            else if (comboBox1.Text == "")
            {
                MessageBox.Show("Выберите валюту!");
            }
            else
            {
                btnCliked = true;

                string valuteCode = "";
                Dictionary<string, double> prices = new Dictionary<string, double>();

                foreach (KeyValuePair<string, string> keyValue in valutes)
                {
                    if (keyValue.Value == comboBox1.Text)
                    {
                        valuteCode = keyValue.Key;
                    }
                }

                string url = "http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1=" + 
                               dateTimePicker2.Value.ToString("dd/MM/yyyy").Replace('.', '/') +
                              "&date_req2=" + dateTimePicker3.Value.ToString("dd/MM/yyyy").Replace('.', '/') + 
                              "&VAL_NM_RQ=" + valuteCode;

                XmlTextReader reader = new XmlTextReader(url);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (reader.Name == "Record")
                            {
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "Date")
                                        {
                                            string date = reader.Value;
                                            while (reader.Name != "Value")
                                            {
                                                reader.Read();
                                            }
                                            reader.Read();

                                            prices.Add(date, Convert.ToDouble(reader.Value));
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                reader.Close();

                if (btnCliked)
                {
                    chart1.Series["Series1"].Points.Clear();

                }

                double min = prices.Min(s => s.Value);
                double max = prices.Max(s => s.Value);
                chart1.ChartAreas[0].AxisY.Minimum = min;
                chart1.ChartAreas[0].AxisY.Maximum = max;

                foreach (KeyValuePair<string, double> keyValue in prices)
                {
                    chart1.Series["Series1"].Points.AddXY(keyValue.Key, keyValue.Value);
                }
                chart1.Visible = true;
            }
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            compareDateToPresent(dateTimePicker1);
        }

        private void dateTimePicker2_CloseUp(object sender, EventArgs e)
        {
            compareDateToPresent(dateTimePicker2);
        }

        private void dateTimePicker3_CloseUp(object sender, EventArgs e)
        {
            compareDateToPresent(dateTimePicker3);

            if (DateTime.Compare(dateTimePicker2.Value.Date, dateTimePicker3.Value.Date) > 0)
            {
                dateTimePicker3.Value = DateTime.Now;
                MessageBox.Show("Ошибка в выборе периода!\nВторая дата должны быть старше первой!\nВыберите корректный период.");
            }
        }
    }
}