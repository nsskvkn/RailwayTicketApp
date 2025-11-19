using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class EditWagonForm : Form  // Успадковується від Form
    {
        private Wagon wagon;
        private RailwayDbContext dbContext;

        public EditWagonForm(Wagon existingWagon, RailwayDbContext context)
        {
            wagon = existingWagon;
            dbContext = context;
            InitializeComponent();
            if (wagon != null)
            {
                PopulateFields();
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(460, 400);
            this.Text = wagon == null ? "Додати вагон" : "Редагувати вагон";
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
                "Тип вагону:", "Потяг:", "Загальна кількість місць:"
            };

            int labelWidth = 130;
            int inputWidth = 200;
            int rowHeight = 45;
            int topMargin = 20;

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

                if (i == 0) // ComboBox для типу вагону
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbWagonType",
                        Location = new Point(160, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmb.Items.AddRange(new[] { "Плацкарт", "Купе", "Люкс" });
                    panelForm.Controls.Add(cmb);
                }
                else if (i == 1) // ComboBox для потяга
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbTrain",
                        Location = new Point(160, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    // Завантаження списку потягів
                    var trains = dbContext.Trains.ToList();
                    foreach (var t in trains)
                    {
                        cmb.Items.Add(new { Text = $"{t.TrainNumber} - {t.TrainName}", Value = t.TrainId });
                    }
                    panelForm.Controls.Add(cmb);
                }
                else if (i == 2) // Кількість місць
                {
                    var txt = new TextBox
                    {
                        Name = "txtSeats",
                        Location = new Point(160, topMargin + i * rowHeight),
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
            btnOK.Click += (s, e) => SaveWagon(
                panelForm.Controls["cmbWagonType"] as ComboBox,
                panelForm.Controls["cmbTrain"] as ComboBox,
                panelForm.Controls["txtSeats"] as TextBox);

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
            var cmbWagonType = this.Controls.Find("cmbWagonType", true)[0] as ComboBox;
            var cmbTrain = this.Controls.Find("cmbTrain", true)[0] as ComboBox;
            var txtSeats = this.Controls.Find("txtSeats", true)[0] as TextBox;

            cmbWagonType.SelectedItem = wagon.WagonType;
            var trains = dbContext.Trains.ToList();
            var selectedTrain = trains.FirstOrDefault(t => t.TrainId == wagon.TrainId);
            if (selectedTrain != null)
            {
                cmbTrain.SelectedItem = new { Text = $"{selectedTrain.TrainNumber} - {selectedTrain.TrainName}", Value = selectedTrain.TrainId };
            }
            txtSeats.Text = wagon.TotalSeats.ToString();
        }

        private void SaveWagon(ComboBox cmbWagonType, ComboBox cmbTrain, TextBox txtSeats)
        {
            if (cmbWagonType.SelectedItem == null || cmbTrain.SelectedItem == null || string.IsNullOrWhiteSpace(txtSeats.Text))
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (wagon == null)
            {
                wagon = new Wagon();
                dbContext.Wagons.Add(wagon);
            }

            wagon.WagonType = cmbWagonType.SelectedItem.ToString();
            var selectedTrain = cmbTrain.SelectedItem as dynamic;
            wagon.TrainId = selectedTrain.Value;
            wagon.TotalSeats = int.Parse(txtSeats.Text);
            // BookedSeats залишається як є, якщо це редагування

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
