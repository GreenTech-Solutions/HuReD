﻿using System;
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
using System.Windows.Shapes;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Vacation.xaml
    /// </summary>
    public partial class Vacation : Window
    {
        public Vacation(ПриказОтпуск order = null)
        {
            InitializeComponent();

            rbEveryYear.IsChecked = true;

            if (order != null)
            {
                dpBegin.Text = order.НачалоОтпуска.ToString();
                dpEnd.Text = order.КонецОтпуска.ToString();
                if (order.Вид == "Ежегодный")
                {
                    rbEveryYear.IsChecked = true;
                }
                else if (order.Вид == "Единоразовый")
                {
                    rbOnce.IsChecked = true;
                }
                else
                {
                    rbOther.IsChecked = true;
                    tbДругое.Text = order.Вид;
                }
            }
        }


        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            // TODO Реализация функции печати
            Functions.Print();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO Добваить логику при сохранении
            string vacationType = "";
            if (rbEveryYear.IsChecked == true)
            {
                vacationType = rbEveryYear.Content.ToString();
            }
            else if (rbOnce.IsChecked == true)
            {
                vacationType = rbOnce.Content.ToString();
            }
            else if (rbOther.IsChecked == true)
            {
                vacationType = tbДругое.Text;
            }

            var order = new ПриказОтпуск()
            {
                НачалоОтпуска = dpBegin.DisplayDate,
                КонецОтпуска = dpEnd.DisplayDate,
                Вид = vacationType
            };
            Tag = order;

            DialogResult = true;
            Close();
        }
    }
}
