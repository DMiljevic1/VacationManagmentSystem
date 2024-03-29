﻿using DomainModel.DtoModels;
using System.Security.Cryptography;
using System.Text;
using VacationDaysCalculatorWebAPI.Repositories;

namespace VacationDaysCalculatorWebAPI.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string GetUserPassword(int userId)
        {
            var user = _userRepository.GetUser(userId);
            return user.Password;
        }
        public void ChangePassword(Password password)
        {
            var user = _userRepository.GetUser(password.UserId);
            if(ValidatePassword(password, user.Password))
            {
                user.Password = HashUserPassword(password.NewPassword);
                _userRepository.SaveChanges();
            }
        }
        private bool ValidatePassword(Password password, string currentPassword)
        {
            if (String.IsNullOrWhiteSpace(password.NewPassword))
                return false;
            else if (currentPassword != HashUserPassword(password.OldPassword))
                return false;
            else if (password.NewPassword != password.ConfirmPassword)
                return false;
            else
                return true;
        }
        private string HashUserPassword(string plainTextPassword)
        {
            string hashedPassword;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                hashedPassword = GetHash(sha256Hash, plainTextPassword);
            }
            return hashedPassword;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
