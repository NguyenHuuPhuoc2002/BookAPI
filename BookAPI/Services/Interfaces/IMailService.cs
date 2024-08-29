using BookAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IMailService
    {
        Task<bool> SendEmail(MailRequest request);
    }
}
