using System;
using System.Drawing;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class EditTrainForm : Form
    {
        private Train train;
        private RailwayDbContext dbContext;

        public EditTrainForm(Train existingTrain, RailwayDbContext context)
        {
            train = existingTrain;
            dbContext = context;
            InitializeComponent();
            if (train != null)
            {
                PopulateFields();
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(460, 500); // ✅ Збільшено висоту
            this.Text = train == null ? "Додати потяг" : "Редагувати потяг";
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Заголовок
            var lblTitle = new Label
            {
                Text = this.Text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40,
                BackColor = Color.FromArgb(230, 230, 230)
            };
            this.Controls.Add(lblTitle);

            // Панель форми
            var panelForm = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(panelForm);

            // Елементи форми
            var labels = new[]
            {
                "Номер:", "Назва:", "Відправлення:", "Прибуття:", "Час відпр.:", "Час приб.:", "Ціна:"
            };

            int labelWidth = 90;
            int inputWidth = 280; // ✅ Зменшено ширину полів
            int rowHeight = 45;
            int topMargin = 15;

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label
                {
                    Text = labels[i],
                    Location = new Point(20, topMargin + i * rowHeight),
                    Size = new Size(labelWidth, 20),
                    ForeColor = Color.FromArgb(50, 50, 50)
                };
                panelForm.Controls.Add(lbl);

                if (i < 4) // Текстові поля
                {
                    var txt = new TextBox
                    {
                        Name = $"txt{i}",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panelForm.Controls.Add(txt);
                }
                else if (i < 6) // DateTimePicker
                {
                    var dtp = new DateTimePicker
                    {
                        Name = $"dtp{i}",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth - 30, 20), // ✅ Ширина менша
                        Format = DateTimePickerFormat.Custom,
                        CustomFormat = "dd.MM.yyyy HH:mm"
                    };
                    panelForm.Controls.Add(dtp);
                }
                else // Ціна
                {
                    var txt = new TextBox
                    {
                        Name = $"txt{i}",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panelForm.Controls.Add(txt);
                }
            }

            // Кнопки — в окремій панелі знизу
            var panelButtons = new Panel
            {
                Location = new Point(10, panelForm.Bottom + 10),
                Size = new Size(this.ClientSize.Width - 20, 40),
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelButtons);

            var btnOK = new Button
            {
                Text = "Зберегти",
                Location = new Point(100, 5),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOK.Click += (s, e) => SaveTrain(
                panelForm.Controls["txt0"] as TextBox,
                panelForm.Controls["txt1"] as TextBox,
                panelForm.Controls["txt2"] as TextBox,
                panelForm.Controls["txt3"] as TextBox,
                panelForm.Controls["dtp4"] as DateTimePicker,
                panelForm.Controls["dtp5"] as DateTimePicker,
                panelForm.Controls["txt6"] as TextBox);

            var btnCancel = new Button
            {
                Text = "Скасувати",
                Location = new Point(220, 5),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            panelButtons.Controls.AddRange(new Control[] { btnOK, btnCancel });
        }

        private void PopulateFields()
        {
            var txtNumber = this.Controls.Find("txt0", true)[0] as TextBox;
            var txtName = this.Controls.Find("txt1", true)[0] as TextBox;
            var txtDeparture = this.Controls.Find("txt2", true)[0] as TextBox;
            var txtArrival = this.Controls.Find("txt3", true)[0] as TextBox;
            var dtpDeparture = this.Controls.Find("dtp4", true)[0] as DateTimePicker;
            var dtpArrival = this.Controls.Find("dtp5", true)[0] as DateTimePicker;
            var txtPrice = this.Controls.Find("txt6", true)[0] as TextBox;

            txtNumber.Text = train.TrainNumber;
            txtName.Text = train.TrainName;
            txtDeparture.Text = train.DepartureStation;
            txtArrival.Text = train.ArrivalStation;
            dtpDeparture.Value = train.DepartureTime;
            dtpArrival.Value = train.ArrivalTime;
            txtPrice.Text = train.BasePrice.ToString("F2");
        }

        private void SaveTrain(TextBox txtNumber, TextBox txtName, TextBox txtDeparture, TextBox txtArrival, DateTimePicker dtpDeparture, DateTimePicker dtpArrival, TextBox txtPrice)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtDeparture.Text) ||
                string.IsNullOrWhiteSpace(txtArrival.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (train == null)
            {
                train = new Train();
                dbContext.Trains.Add(train);
            }

            train.TrainNumber = txtNumber.Text.Trim();
            train.TrainName = txtName.Text.Trim();
            train.DepartureStation = txtDeparture.Text.Trim();
            train.ArrivalStation = txtArrival.Text.Trim();
            train.DepartureTime = dtpDeparture.Value;
            train.ArrivalTime = dtpArrival.Value;
            train.BasePrice = decimal.Parse(txtPrice.Text);

            try
            {
                dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}