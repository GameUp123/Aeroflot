using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Aeroflot;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Aeroflot
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
           
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "flyWayDataSet.Airport". При необходимости она может быть перемещена или удалена.
            
            // Этот метод может быть использован для дополнительной настройки при загрузке формы
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void ExitButton_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }

        private void airportBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            

        }
    }
}
