﻿using System;
using System.IO;
using System.Windows;
using Hured.DBModel;
using Hured.Tables_templates;
using Microsoft.Win32;

namespace Hured
{
    public class OrderInfo
    {
        public OrderInfo(string номер, string фио, string тип, string дата)
        {
            Номер = номер;
            Дата = дата;
            Тип = тип;
            Фио = фио;
        }

        public string Номер { get; set; }

        public string Дата { get; set; }

        public string Фио { get; set; }

        public string Тип { get; set; }

        public int Id { get; set; }

        public OrderType OrderType { get; set; }

        public Type Type { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders
    {
        public Orders()
        {
            InitializeComponent();

            SyncOrders();
        }

        void SyncOrders()
        {
            LvOrders.Items.Clear();

            Controller.OpenConnection();

            var приказыПриём = Controller.Select< ПриказПриём>( e => e != null);
            foreach (var e in приказыПриём)
            {
                LvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Приём", e.Дата.ToShortDateString())
                {
                    Id = e.ПриказПриёмId,
                    OrderType = OrderType.Recruitment,
                    Type = e.GetType()
                });
            }

            var приказыУвольнение = Controller.Select<ПриказУвольнение>( e => e != null);
            foreach (var e in приказыУвольнение)
            {
                LvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Увольнение", e.Дата.ToShortDateString())
                {
                    Id = e.ПриказУвольнениеId,
                    OrderType = OrderType.Dismissal,
                    Type = e.GetType()
                });
            }

            var приказыОтпуск = Controller.Select<ПриказОтпуск>( e => e != null);
            foreach (var e in приказыОтпуск)
            {
                LvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Отпуск", e.Дата.ToShortDateString())
                {
                    Id = e.ПриказОтпускId,
                    OrderType = OrderType.Vacation,
                    Type = e.GetType()
                });
            }

            var приказыКомандировка = Controller.Select<ПриказКомандировка>(e => e != null);
            foreach (var e in приказыКомандировка)
            {
                LvOrders.Items.Add(new OrderInfo(e.Номер,
                    e.Сотрудник.ОсновнаяИнформация.Фамилия + " " +
                    e.Сотрудник.ОсновнаяИнформация.Имя + " " +
                    e.Сотрудник.ОсновнаяИнформация.Отчество, "Командировка", e.Дата.ToShortDateString())
                {
                    Id = e.ПриказКомандировкаId,
                    OrderType = OrderType.BusinessTrip,
                    Type = e.GetType()
                });
            }

            Controller.CloseConnection();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Order();
            w.ShowDialog();

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                var w = new Order(item);
                w.ShowDialog();
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void BOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            Controller.OpenConnection();
            if (item != null)
            {
                WordDocument document;
                switch (item.OrderType)
                {
                    case OrderType.Recruitment:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
                            q => q.ПриказПриёмId == item.Id));
                        break;
                    case OrderType.Dismissal:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
                            q => q.ПриказУвольнениеId == item.Id));
                        break;
                    case OrderType.Vacation:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
                            q => q.ПриказОтпускId == item.Id));
                        break;
                    case OrderType.BusinessTrip:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
                            q => q.ПриказКомандировкаId == item.Id));
                        break;
                    default:
                        document = null;
                        break;
                }
                var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                if (document != null)
                {
                    document.Save(savePath);
                    document.Path = savePath;
                    document.Close();
                    document.OpenWithWord();
                }
                Controller.CloseConnection();
            }
        }

        private void BSave_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            if (item == null) return;
            var sfd = new SaveFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                    //initialDirectory == null
                    //    ? Directory.GetCurrentDirectory() + @"\Documents"
                    //    : System.IO.Path.GetDirectoryName(initialDirectory),
                Filter = "Word Document | *.docx | Все файлы (*.*)|*.*",
                FileName = "Новый приказ"
            };

            sfd.ShowDialog();


            WordDocument document;
            Controller.OpenConnection();
            switch (item.OrderType)
            {
                case OrderType.Recruitment:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
                        q => q.ПриказПриёмId == item.Id));
                    break;
                case OrderType.Dismissal:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
                        q => q.ПриказУвольнениеId == item.Id));
                    break;
                case OrderType.Vacation:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
                        q => q.ПриказОтпускId == item.Id));
                    break;
                case OrderType.BusinessTrip:
                    document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
                        q => q.ПриказКомандировкаId == item.Id));
                    break;
                default:
                    document = null;
                    break;
            }


            if (document != null)
            {
                document.Save(sfd.FileName,false);

                document.Close();
            }
            Controller.CloseConnection();
        }

        private void BPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                WordDocument document;
                Controller.OpenConnection();
                switch (item.OrderType)
                {
                    case OrderType.Recruitment:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказПриём>(
                            q => q.ПриказПриёмId == item.Id));
                        break;
                    case OrderType.Dismissal:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказУвольнение>(
                            q => q.ПриказУвольнениеId == item.Id));
                        break;
                    case OrderType.Vacation:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказОтпуск>(
                            q => q.ПриказОтпускId == item.Id));
                        break;
                    case OrderType.BusinessTrip:
                        document = Functions.CreateOrder(item.OrderType, Controller.Find<ПриказКомандировка>(
                            q => q.ПриказКомандировкаId == item.Id));
                        break;
                    default:
                        document = null;
                        break;
                }
                var savePath = Directory.GetCurrentDirectory() + @"\Temp.docx";
                if (document != null)
                {
                    document.Save(savePath,false);
                    document.Path = savePath;
                    document.Print();
                    document.Close();
                }
                Controller.CloseConnection();
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;


            var item = LvOrders.SelectedItem as OrderInfo;
            if (item != null)
            {
                if (item.Тип == "Приём")
                {
                    Controller.OpenConnection();
                    Controller.Remove<ПриказПриём>(q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Увольнение")
                {
                    Controller.OpenConnection();
                    Controller.Remove<ПриказУвольнение>( q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Отпуск")
                {
                    Controller.OpenConnection();
                    Controller.Remove<ПриказОтпуск>( q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
                else if (item.Тип == "Командировка")
                {
                    Controller.OpenConnection();
                    Controller.Remove<ПриказКомандировка>( q => q.Номер == item.Номер);
                    Controller.CloseConnection();
                }
            }

            IsHitTestVisible = true;

            SyncOrders();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
