using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp9.Data;
using WinFormsApp9.Models;

namespace WinFormsApp9
{
    public partial class Form1 : Form
    {
        private readonly TicketRepository _repo = new TicketRepository();
        private readonly string _dataFile;

        public Form1()
        {
            InitializeComponent();
            _dataFile = Path.Combine(Application.StartupPath, "tickets.csv");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load existing tickets
            _repo.LoadFromCsv(_dataFile);
            UpdateTicketList();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _repo.SaveToCsv(_dataFile);
            }
            catch
            {
                // ignore save errors for prototype
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out var message))
            {
                MessageBox.Show(message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ticket = new Ticket
            {
                From = comboFrom.Text,
                To = comboTo.Text,
                Date = dtpDate.Value.Date,
                Time = txtTime.Text,
                Seat = txtSeat.Text,
                Name = txtName.Text,
                FIN = txtFIN.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text
            };

            _repo.Add(ticket);
            _repo.SaveToCsv(_dataFile);
            UpdateTicketList();
            ClearPersonInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var sel = listTickets.SelectedItem as Ticket;
            if (sel == null)
            {
                MessageBox.Show("Silmek üçün bir bilet seçin.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Seçilen bileti silmək istədiyinizə əminsiniz?", "Təsdiq", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            _repo.Remove(sel.Id);
            _repo.SaveToCsv(_dataFile);
            UpdateTicketList();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            // swap the From and To selections
            var a = comboFrom.Text;
            var b = comboTo.Text;
            comboFrom.Text = b;
            comboTo.Text = a;
        }

        private void UpdateTicketList()
        {
            listTickets.Items.Clear();
            foreach (var t in _repo.Tickets)
            {
                listTickets.Items.Add(t);
            }
        }

        private bool ValidateInputs(out string message)
        {
            if (string.IsNullOrWhiteSpace(comboFrom.Text))
            {
                message = "Zəhmət olmasa 'Haradan' seçin.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(comboTo.Text))
            {
                message = "Zəhmət olmasa 'Haraya' seçin.";
                return false;
            }
            if (comboFrom.Text == comboTo.Text)
            {
                message = "Haradan və Haraya eyni olmamalıdır.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                message = "Ad və soyad daxil edin.";
                return false;
            }
            // FIN must be exactly 7 characters (any characters allowed)
            if (string.IsNullOrEmpty(txtFIN.Text) || txtFIN.Text.Length != 7)
            {
                message = "FIN 7 simvoldan ibarət olmalıdır.";
                return false;
            }

            // Phone should be complete per mask
            if (!txtPhone.MaskFull)
            {
                message = "Telefon nömrəsini tam daxil edin.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private void ClearPersonInputs()
        {
            txtName.Text = string.Empty;
            txtFIN.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
