using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Context;
using DataLayer.Context.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Objects;

namespace Services.Services
{
    public interface IUserServcie
    {
        public Task<UserDto> GetByUserNamePassword(string username,string password);
        public Task<IEnumerable<UserDto>> GetAll();
        public Task<UserDto> GetById(int memberId);
        public Task<UserDto> GetByNationalCode(string nationalCode);
        public Task<UserDto> GetCurrentUser();
        public Task<UserDto> AddEdit(UserDto userDto, bool isAdd);
        public Task<bool> Remove(UserDto userDto);
        public int CurrentUserId { get;  }
        public string CurrentUserIP { get;  }
    }
    public class UserService:IUserServcie
    {
       

        public int CurrentUserId {
            get
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return -1;
                }
                return int.Parse(userId);
            }
        }

        public string CurrentUserIP {
            get
            {
                var userIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                return userIp.ToString();
            }
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<User> _userDbSet;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUnitOfWork unitOfWork 
            ,IMapper mapper,IPasswordHasher passwordHasher, ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userDbSet = unitOfWork.Set<User>();
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Remove(UserDto userDto)
        {
            try
            {
                userDto.IsDeleted = true;
                var deleteResult = await AddEdit(userDto, false);
                return deleteResult!=null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<UserDto> GetById(int memberId)
        {
            try
            {
                var user =await _userDbSet.FindAsync(memberId);
                var userDto = _mapper.Map<User,UserDto>(user);
                return userDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<UserDto> GetByNationalCode(string nationalCode)
        {
            try
            {
                var user = await _userDbSet.Where(u=>!u.IsDeleted).FirstOrDefaultAsync(u => u.NationalNum == nationalCode);
                return _mapper.Map<User, UserDto>(user);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<UserDto> GetCurrentUser()
        {
            try
            {
                var userId =  _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                var user = await _userDbSet.FindAsync(int.Parse(userId));
                return _mapper.Map<User, UserDto>(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<UserDto> AddEdit(UserDto userDto, bool isAdd)
        {

            try
            {
                
                if (isAdd)
                {var user = _mapper.Map<UserDto, User>(userDto);
                    user.CreateDate = DateTime.Now;
                    user.CreatedBy = CurrentUserId;
                    user.Username = userDto.NationalNum;
                    user.Password = _passwordHasher.Hash(userDto.NationalNum);
                    user.UserIp = CurrentUserIP;
                    //user.UserRoles.Add();
                    var addResult = await _userDbSet.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    return _mapper.Map<User, UserDto>(user);

                }
                else
                {
                    var editedUser = await _userDbSet.FindAsync(userDto.Id);
                    var user = _mapper.Map<UserDto, User>(userDto,editedUser);
                    user.LastUpdateDate = DateTime.Now;
                    user.LastUpdateBy = CurrentUserId;
                    var editResult = _userDbSet.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return _mapper.Map<User, UserDto>(editResult.Entity);
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        

        public async Task<UserDto> GetByUserNamePassword(string username, string password)
        {
            try
            {
                var user = await _userDbSet.Where(u => !u.IsDeleted).FirstOrDefaultAsync(u => u.Username == username);
                if (user==null)
                {
                    var hashPass = _passwordHasher.Hash(password);

                    _logger.LogInformation($"User with username {username} not found");
                    return null;

                }
                var passIsMatch = _passwordHasher.Check(user.Password, password);
                UserDto userDto = null;
                if (passIsMatch.Verified)
                {
                    userDto = _mapper.Map<User, UserDto>(user);
                }
                return userDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            try
            {
                var users = _userDbSet.Where(u=>!u.IsDeleted).OrderByDescending(u => u.Id);
                var usersDto = _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users);
                return usersDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

        }
    }
}
