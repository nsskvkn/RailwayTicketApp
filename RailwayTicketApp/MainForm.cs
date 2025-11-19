using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Forms;
using System.IO;

namespace RailwayTicketApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // Встановлюємо DataDirectory у AppData
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dbDir = Path.Combine(appDataPath, "RailwayTicketApp");
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", dbDir);

            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = "Залізнична система";
            this.StartPosition = FormStartPosition.CenterScreen;

            // ✅ Встановлюємо, що ця форма є контейнером для Mdi
            this.IsMdiContainer = true;
        }

        private void SetupUI()
        {
            this.Text = "Залізнична система";
            this.BackColor = Color.FromArgb(240, 245, 249);

            var menu = new MenuStrip();
            var trainMenu = new ToolStripMenuItem("Потяги");
            var wagonMenu = new ToolStripMenuItem("Вагони");
            var bookingMenu = new ToolStripMenuItem("Бронювання");
            var searchMenu = new ToolStripMenuItem("Пошук");

            trainMenu.DropDownItems.Add("Додати", null, (s, e) => OpenForm(new TrainManagementForm()));
            trainMenu.DropDownItems.Add("Переглянути", null, (s, e) => OpenForm(new TrainManagementForm()));

            wagonMenu.DropDownItems.Add("Додати/Видалити", null, (s, e) => OpenForm(new WagonManagementForm()));

            bookingMenu.DropDownItems.Add("Управління", null, (s, e) => OpenForm(new BookingManagementForm()));

            searchMenu.DropDownItems.Add("Пошук", null, (s, e) => OpenForm(new SearchForm()));

            menu.Items.AddRange(new ToolStripItem[] { trainMenu, wagonMenu, bookingMenu, searchMenu });
            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }

        private void OpenForm(Form form)
        {
            // ✅ Тепер це MdiContainer, тому можна встановити MdiParent
            form.MdiParent = this;
            form.Show();
        }
    }
}