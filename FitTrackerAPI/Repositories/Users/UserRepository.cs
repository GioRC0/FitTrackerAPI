using FitTrackerAPI.Data;
using FitTrackerAPI.Models.UserInfo;
using MongoDB.Driver;

namespace FitTrackerAPI.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        // Busca el usuario y sus tokens de refresco, esencial para la autenticación
        return _users.Find(u => u.Username == username).SingleOrDefaultAsync();
    }

    public async Task AddUserAsync(User user)
    {
        // Se asume que el usuario y el perfil están vinculados en la capa de servicio
        await _users.InsertOneAsync(user);
    }

    public Task<User?> GetUserByIdAsync(string id)
    {
        // Útil para buscar al usuario después de validar un Refresh Token
        return _users.Find(u => u.Id == id).SingleOrDefaultAsync();
    }

    public async Task MarkUserAsVerifiedAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Set(u => u.IsVerified, true);
        await _users.UpdateOneAsync(filter, update);
    }
    
    public async Task UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _users.ReplaceOneAsync(filter, user);
    }
    
    public Task<User?> GetUserByRefreshTokenAsync(string token)
    {
        var filter = Builders<User>.Filter.ElemMatch(u => u.RefreshTokens, rt => rt.Token == token);
        return _users.Find(filter).FirstOrDefaultAsync();
    }
}