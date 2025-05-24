using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using UtazasNaplozas.Data;
using UtazasNaplozas.Repository.IRepository;

namespace UtazasNaplozas.Components.Pages;
public partial class Home
{
	[Inject]
	private IJourneyRepository _journeyRepository { get; set; }
	[Inject]
	private AuthenticationStateProvider _authStateProvider { get; set; }
	[Inject]
	private IJSRuntime _js { get; set; }
	private bool isProcessing { get; set; } = false;
	private List<JourneyDto> _journeys { get; set; } = new();
	private bool _showModal { get; set; } = false;
	private SubJourneyImage _selectedImage { get; set; }

	protected override async Task OnInitializedAsync()
	{
		isProcessing = true;
		await GetAllJourney();
		StateHasChanged();
		isProcessing = false;
	}

	private async Task DeleteJourney(JourneyDto journey)
	{
		await _journeyRepository.DeleteAsync(journey.Id);
		await OnInitializedAsync();
	}
	private async Task GetAllJourney()
	{
		_journeys = (await _journeyRepository.GetAllAsync()).ToList();
	}

	private string GetImage(SubJourneyImage image)
	{
		string filename = image.FileName;
		string userEmail = _authStateProvider.GetAuthenticationStateAsync().Result.User.Identity.Name;
		string path = Path.Combine("Images\\", userEmail, filename);
		return path;
	}

	private async Task LoadMap(JourneyDto journeyDto)
	{
		if (!journeyDto.IsMapLoaded)
		{
			await _js.InvokeVoidAsync("loadMapWithId", $"map-{journeyDto.Id}");
			var serializableCoords = journeyDto.LatLngs.Select(c => new { Lat = c.Lat, Lng = c.Lng }).ToList();
			await _js.InvokeVoidAsync("addMarkers", serializableCoords);
			journeyDto.IsMapLoaded = true;

		}

	}
}
