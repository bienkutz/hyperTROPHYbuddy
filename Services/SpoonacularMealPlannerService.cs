using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace hyperTROPHYbuddy.Services
{
    public class SpoonacularMealPlannerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SpoonacularMealPlannerService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        // Existing method omitted for brevity...

        public async Task<RecipeResult> GenerateSingleMealAsync(
            int mealCalories,
            string mealType,
            string diet,
            List<string> allergies,
            List<int> excludeIds = null)
        {
            var url = $"https://api.spoonacular.com/recipes/complexSearch?number=10&addRecipeNutrition=true&apiKey={_apiKey}";

            if (!string.IsNullOrWhiteSpace(diet))
                url += $"&diet={diet}";

            // Only add type if mealType is not null or empty
            if (!string.IsNullOrWhiteSpace(mealType))
                url += $"&type={mealType}";

            if (allergies != null && allergies.Count > 0)
                url += $"&excludeIngredients={string.Join(",", allergies)}";

            int minCalories = (int)(mealCalories * 0.9);
            int maxCalories = (int)(mealCalories * 1.1);
            url += $"&minCalories={minCalories}&maxCalories={maxCalories}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var searchResult = JsonSerializer.Deserialize<ComplexSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var candidates = searchResult.Results;
            if (excludeIds != null && excludeIds.Count > 0)
                candidates = candidates.Where(r => !excludeIds.Contains(r.Id)).ToList();

            var recipe = candidates.FirstOrDefault();
            if (recipe == null)
                return null;

            var detailsUrl = $"https://api.spoonacular.com/recipes/{recipe.Id}/information?includeNutrition=true&apiKey={_apiKey}";
            var detailsResponse = await _httpClient.GetAsync(detailsUrl);
            detailsResponse.EnsureSuccessStatusCode();
            var detailsJson = await detailsResponse.Content.ReadAsStringAsync();

            var details = JsonSerializer.Deserialize<RecipeResult>(detailsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return details;
        }

        public async Task<List<RecipeSummary>> GetRecipeBatchAsync(
    int mealCalories,
    string mealType,
    string diet,
    List<string> allergies,
    int batchSize = 10)
        {
            var url = $"https://api.spoonacular.com/recipes/complexSearch?number={batchSize}&addRecipeNutrition=true&apiKey={_apiKey}";

            if (!string.IsNullOrWhiteSpace(diet))
                url += $"&diet={diet}";
            if (!string.IsNullOrWhiteSpace(mealType))
                url += $"&type={mealType}";
            if (allergies != null && allergies.Count > 0)
                url += $"&excludeIngredients={string.Join(",", allergies)}";

            int minCalories = (int)(mealCalories * 0.9);
            int maxCalories = (int)(mealCalories * 1.1);
            url += $"&minCalories={minCalories}&maxCalories={maxCalories}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var searchResult = JsonSerializer.Deserialize<ComplexSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return searchResult.Results ?? new List<RecipeSummary>();
        }

        public async Task<RecipeResult> GetRecipeDetailsAsync(int recipeId)
        {
            var detailsUrl = $"https://api.spoonacular.com/recipes/{recipeId}/information?includeNutrition=true&apiKey={_apiKey}";
            var detailsResponse = await _httpClient.GetAsync(detailsUrl);
            detailsResponse.EnsureSuccessStatusCode();
            var detailsJson = await detailsResponse.Content.ReadAsStringAsync();

            var details = JsonSerializer.Deserialize<RecipeResult>(detailsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return details;
        }

        // Models for complex search and recipe details
        public class ComplexSearchResult
        {
            public List<RecipeSummary> Results { get; set; }
        }

        public class RecipeSummary
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Image { get; set; }
        }

        public class RecipeResult
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string ImageType { get; set; }
            public string SourceUrl { get; set; }
            public Nutrition Nutrition { get; set; }
            public List<ExtendedIngredient> ExtendedIngredients { get; set; }
        }

        public class Nutrition
        {
            public List<Nutrient> Nutrients { get; set; }
        }

        public class Nutrient
        {
            public string Name { get; set; }
            public double Amount { get; set; }
            public string Unit { get; set; }
        }

        public class ExtendedIngredient
        {
            public string OriginalString { get; set; }
        }
    }
}