using System;
using System.Linq;
using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	List<Produto> todosProdutos = new List<Produto>();

	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		lista.Clear();

		// Use a propriedade correta do seu App (App.Db ou App.Database)
		todosProdutos = await App.Db.GetAll();
		AtualizarListas("Todos");
	}

	private void pckFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e)
	{
		string categoriaSelecionada = pckFiltroCategoria.SelectedItem?.ToString() ?? "Todos";
		AtualizarListas(categoriaSelecionada);
	}

	private void AtualizarListas(string filtro)
	{
		IEnumerable<Produto> produtosFiltrados = filtro == "Todos"
			? todosProdutos
			: todosProdutos.Where(p => p.Categoria == filtro);

		// Atualiza a ListView/CollectionView existente de produtos
		lista.Clear();
		foreach (var p in produtosFiltrados) lista.Add(p);


		// Relatório por categoria — exige cvRelatorioCategoria no XAML
		var relatorio = todosProdutos
			.GroupBy(p => p.Categoria)
			.Select(g => new
			{
				Categoria = string.IsNullOrEmpty(g.Key) ? "Sem Categoria" : g.Key,
				TotalGasto = g.Sum(p => p.Preco * p.Quantidade)
			})
			.ToList();

		// Esta linha requer um elemento com x:Name="cvRelatorioCategoria" no XAML
		cvRelatorioCategoria.ItemsSource = relatorio;
	}

	// Handler para o SearchBar TextChanged
	private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
	{
		var texto = e.NewTextValue ?? string.Empty;
		var filtrado = todosProdutos.Where(p => !string.IsNullOrEmpty(p.Descricao) && p.Descricao.Contains(texto, StringComparison.OrdinalIgnoreCase));
		lista.Clear();
		foreach (var p in filtrado) lista.Add(p);
	}

	// Handler para o Pull to Refresh
	private async void lst_produtos_Refreshing(object sender, EventArgs e)
	{
		// Recarrega os dados da fonte
		todosProdutos = await App.Db.GetAll();
		AtualizarListas(pckFiltroCategoria.SelectedItem?.ToString() ?? "Todos");
		lst_produtos.IsRefreshing = false;
	}

	// Handler quando um item é selecionado
	private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		if (e.SelectedItem == null) return;
		// Aqui você pode navegar para a página de detalhes
		lst_produtos.SelectedItem = null; // Deseleciona para permitir nova seleção
	}

	// Handler para o MenuItem (remover)
	private async void MenuItem_Clicked(object sender, EventArgs e)
	{
		if (sender is MenuItem mi && mi.BindingContext is Produto p)
		{
			bool confirmar = await DisplayAlert("Remover", $"Remover {p.Descricao}?", "Sim", "Não");
			if (confirmar)
			{
				lista.Remove(p);
				todosProdutos.Remove(p);
			}
		}
	}

	// Handler para o ToolbarItem "Adicionar"
	private async void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			// Navega para a página de Novo Produto
			await Navigation.PushAsync(new NovoProduto());
		}
		catch (Exception ex)
		{
			await DisplayAlert("Erro", ex.Message, "OK");
			
		}
	}

	// Handler para o ToolbarItem "Somar"
	private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		var total = todosProdutos.Sum(p => p.Preco * p.Quantidade);
		await DisplayAlert("Total", $"Total gasto: R$ {total:F2}", "OK");
	}
    private async void ToolbarItem_Relatorio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Relatorio());
    }
}
