using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result> SendAsync<T>(string to, string subject, string templateFileName, T model);
}