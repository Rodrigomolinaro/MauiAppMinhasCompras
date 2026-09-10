using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class Relatorio : ContentPage
{
    List<Produto> todosProdutos = new List<Produto>();

    public Relatorio()
    {
        InitializeComponent();

        // Define as datas padrões ao abrir a tela (ex: últimos 30 dias até hoje)
        dtpDataInicial.Date = DateTime.Now.AddDays(-30);
        dtpDataFinal.Date = DateTime.Now;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        todosProdutos = await App.Db.GetAll(); // Busca do banco
        FiltrarLista();
    }

    private void btnBuscar_Clicked(object sender, EventArgs e)
    {
        FiltrarLista();
    }

    private void FiltrarLista()
    {
        // Pega as datas. O TimeOfDay do dtpDataFinal é ajustado para as 23:59:59 para garantir que pegue o dia final inteiro.
        DateTime dataInicio = (dtpDataInicial.Date ?? DateTime.MinValue).Date;
        DateTime dataFim = (dtpDataFinal.Date ?? DateTime.Now).Date
            .AddHours(23).AddMinutes(59).AddSeconds(59);

        // Aplica o filtro LINQ
        var produtosFiltrados = todosProdutos
            .Where(p => p.DataCadastro >= dataInicio && p.DataCadastro <= dataFim)
            .OrderByDescending(p => p.DataCadastro) // Opcional: Ordena do mais recente pro mais antigo
            .ToList();

        // Atualiza a tela
        lst_relatorio.ItemsSource = produtosFiltrados;
    }
}