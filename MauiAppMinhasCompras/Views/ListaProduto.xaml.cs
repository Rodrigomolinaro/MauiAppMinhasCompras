using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

	protected async override void OnAppearing()
	{
		try
		{
			lista.Clear();

			List<Produto> tmp = await App.Db.GetAll();

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
	}

	private async void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new Views.NovoProduto());
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
	}

	private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			string q = e.NewTextValue;

			lista.Clear();

			List<Produto> tmp = await App.Db.Search(q);

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
	}

	private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		await DisplayAlertAsync("Total dos Produtos", msg, "OK");
	}

	private async void MenuItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			MenuItem selecionado = sender as MenuItem;

			Produto p = selecionado.BindingContext as Produto;

			bool confirm = await DisplayAlertAsync(
				"Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

			if (confirm)
			{
				await App.Db.Delete(p.Id);
				lista.Remove(p);
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
	}

	private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		try
		{
			var selecionado = e.SelectedItem as Produto;
			if (selecionado == null)
				return;

			// Exemplo: navegar para detalhes ou outro comportamento
			await Navigation.PushAsync(new Views.NovoProduto(/* opcional: enviar selecionado */));
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
		finally
		{
			// limpar seleção se usar ListView
			lst_produtos.SelectedItem = null;
		}
	}

	private async void lst_produtos_Refreshing(object sender, EventArgs e)
	{
		try
		{
			lst_produtos.IsRefreshing = true;

			lista.Clear();

			List<Produto> tmp = await App.Db.GetAll();

			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
		finally
		{
			lst_produtos.IsRefreshing = false;
		}
	}
}