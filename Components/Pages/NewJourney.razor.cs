using System.Text;
using System.Text.Json;
using BlazorBootstrap;
using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Maps;
using UtazasNaplozas.Data;
using UtazasNaplozas.Repository.IRepository;

namespace UtazasNaplozas.Components.Pages;

public partial class NewJourney
{
	[Inject]
	private IJourneyRepository _journeyRepository { get; set; }
	[Inject]
	private AuthenticationStateProvider _authStateProvider { get; set; }
	[Inject]
	private ApplicationDbContext _db { get; set; }
	[Inject]
	private NavigationManager _navManager { get; set; }
	[Inject]
	private IJSRuntime _js { get; set; }
	private bool isProcessing { get; set; } = true;
	private Journey journey { get; set; } = new Journey { Id = Guid.NewGuid(), StartDate = DateTime.Now.Date, EndDate = DateTime.Now.Date, SubJourneys = new List<SubJourney>() };
	private SubJourney subJourney { get; set; } = new SubJourney { Id = Guid.NewGuid(), Date = DateTime.Now.Date, Images = new List<SubJourneyImage>() };
	private long maxFileSize = 1024 * 1024 * 10;
	private int maxNumberOfFiles = 3;
	private List<string> errors = new();
	private string filePath = "";
	private List<ToastMessage> messages = new();
	private BlazoredTextEditor QuillHtml = new();
	List<(double lat, double lng)> latlngs { get; set; } = new();
	private bool _isStartingPointSet { get; set; }
	private bool _isDestinationSet { get; set; }
	private bool _isSubJourneyPlaceSet { get; set; }

	protected override async Task OnInitializedAsync()
	{
		isProcessing = true;
		subJourney.JourneyId = journey.Id;
		var userId = _authStateProvider.GetAuthenticationStateAsync().Result.User.Identity.Name;
		var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userId);
		journey.UserId = Guid.Parse(user.Id);
		isProcessing = false;
	}

	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			if (!isProcessing)
			{
				await JsRuntime.InvokeVoidAsync("loadMap");
			}
		}
	}

	private async Task SetStartingPoint(string place)
	{

		try
		{
			var latlng = await GetLocationAsync(place);
			journey.StartingPointLatitude = latlng.lat;
			journey.StartingPointLongitude = latlng.lng;
			_isStartingPointSet = true;
		}
		catch
		{
			errors.Add("A hely nem található");
			_isStartingPointSet = false;
		}

	}

	private async Task DeleteStartingPont()
	{
		await JsRuntime.InvokeVoidAsync("clearMap");
		latlngs = new();
		_isStartingPointSet = false;
	}

	private async Task SetDestination(string place)
	{
		try
		{
			var latlng = await GetLocationAsync(place);
			journey.DestinationLatitude = latlng.lat;
			journey.DestinationLongitude = latlng.lng;
			_isDestinationSet = true;
		}
		catch
		{
			errors.Add("A hely nem található");
			_isDestinationSet = false;
		}
	}

	private async Task DeleteDestination()
	{
		latlngs = latlngs.Take(1).ToList();
		await JsRuntime.InvokeVoidAsync("clearMap");
		var serializableCoords = latlngs.Select(c => new { Lat = c.lat, Lng = c.lng }).ToList();
		await JsRuntime.InvokeVoidAsync("addMarkers", serializableCoords);
		_isDestinationSet = false;
	}

	private async Task SetSubJourneyPlace(string place)
	{
		try
		{
			var latlng = await GetLocationAsync(place);
			subJourney.Latitude = latlng.lat;
			subJourney.Longitude = latlng.lng;
			_isSubJourneyPlaceSet = true;
		}
		catch
		{
			errors.Add("A hely nem található");
			_isSubJourneyPlaceSet = false;
		}
	}

	private async Task DeleteSubJourneyPlace()
	{
		latlngs.Remove(latlngs[latlngs.Count - 2]);
		await JsRuntime.InvokeVoidAsync("clearMap");
		var serializableCoords = latlngs.Select(c => new { Lat = c.lat, Lng = c.lng }).ToList();
		await JsRuntime.InvokeVoidAsync("addMarkers", serializableCoords);
		_isSubJourneyPlaceSet = false;
	}

	private async Task<(double lat, double lng)> GetLocationAsync(string place)
	{
		var apiKey = "AIzaSyBHI8S5NsUk8dQtfWmIRURAdVGm85sVX5U";


		var url = $"https://maps.googleapis.com/maps/api/geocode/json?address=" +
				  $"{Uri.EscapeDataString(place)}&key={apiKey}";

		using var httpClient = new HttpClient();
		var response = await httpClient.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			var jsonDocument = JsonDocument.Parse(json);

			var location = jsonDocument.RootElement
				.GetProperty("results")[0]
				.GetProperty("geometry")
				.GetProperty("location");

			var latitude = location.GetProperty("lat").GetDouble();
			var longitude = location.GetProperty("lng").GetDouble();

			if (latlngs.Contains((latitude, longitude)))
			{
				if (latlngs.Count >= 2)
				{
					latlngs.Insert(latlngs.Count - 1, (latitude, longitude));
				}
				else
				{
					latlngs.Add((latitude, longitude));
				}
			}
			else
			{
				if (latlngs.Count >= 2)
				{
					latlngs.Insert(latlngs.Count - 1, (latitude, longitude));
				}
				else
				{
					latlngs.Add((latitude, longitude));
				}

				var serializableCoords = latlngs.Select(c => new { Lat = c.lat, Lng = c.lng })
												.ToList();
				await JsRuntime.InvokeVoidAsync("addMarkers", serializableCoords);
			}



			return (latitude, longitude);
		}

		return (0, 0);
	}

	public async Task SaveJourney()
	{
		await SaveSubJourney();
		await _journeyRepository.CreateAsync(journey);
		_navManager.NavigateTo("/");

	}

	public async Task SaveSubJourney()
	{
		if (subJourney.Location != null)
		{
			if (subJourney.Description == null)
			{
				subJourney.Description = "";
			}
			subJourney.Description = await GetHTML();
			journey.SubJourneys.Add(subJourney);
		}
		subJourney = new SubJourney { Id = Guid.NewGuid(), JourneyId = journey.Id, Date = DateTime.Now, Order = journey.SubJourneys.Count, Images = new List<SubJourneyImage>() };
		_isSubJourneyPlaceSet = false;
	}

	public async Task<string> GetHTML()
	{
		return await this.QuillHtml.GetHTML();
	}

	public async Task DeleteSubJourney(SubJourney subJourney)
	{
		journey.SubJourneys.Remove(subJourney);
		latlngs.Remove((subJourney.Latitude, subJourney.Longitude));
		await JsRuntime.InvokeVoidAsync("clearMap");
		var serializableCoords = latlngs.Select(c => new { Lat = c.lat, Lng = c.lng }).ToList();
		await JsRuntime.InvokeVoidAsync("addMarkers", serializableCoords);
		_isSubJourneyPlaceSet = false;
	}

	public async Task SaveImages(InputFileChangeEventArgs input)
	{
		errors.Clear();

		if (input.FileCount > maxNumberOfFiles)
		{
			errors.Add($"Több mint {maxNumberOfFiles} fájlt akarsz feltölteni");
			return;
		}

		foreach (var file in input.GetMultipleFiles(maxNumberOfFiles))
		{
			try
			{
				if (file.Size > maxFileSize)
				{
					errors.Add("Túl nagy méret");
				}
				else if (subJourney.Images.Count >= 3)
				{
					errors.Add("Túl sok kép");
				}
				else
				{
					string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.Name));
					string userEmail = _authStateProvider.GetAuthenticationStateAsync().Result.User.Identity.Name;
					string path = Path.Combine("wwwroot\\Images\\", userEmail, newFileName);
					Directory.CreateDirectory(Path.Combine("wwwroot\\Images\\", userEmail));
					await using FileStream fileStream = new(path, FileMode.Create);
					await file.OpenReadStream(maxFileSize).CopyToAsync(fileStream);
					filePath = path;
					subJourney.Images.Add(new() { Id = Guid.NewGuid().ToString(), SubJourneyId = subJourney.Id, FileName = newFileName });
				}
			}
			catch (Exception ex)
			{
				errors.Add($"{ex.Message}");
			}
		}
	}

	private string GetImage(SubJourneyImage image)
	{
		string filename = image.FileName;
		string userEmail = _authStateProvider.GetAuthenticationStateAsync().Result.User.Identity.Name;
		string path = Path.Combine("Images\\", userEmail, filename);
		return path;
	}

	private void ShowMessage(ToastType type, List<string> errors){
		messages.Add(CreateToastMessage(type, errors));
		this.errors.Clear();
	}

	private ToastMessage CreateToastMessage(ToastType type, List<string> errors)
	{
		StringBuilder sb = new();
		foreach (var error in errors)
		{
			sb.AppendLine(error);
		}
		errors.Clear();
		string message = sb.ToString();
		return new ToastMessage
		{
			Type = type,
			Title = "Hiba!",
			Message = message
		};
	}
}
