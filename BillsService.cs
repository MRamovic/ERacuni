using ERacuniNovi.Server.Context;
using ERacuniNovi.Shared.Konvertor;
using ERacuniNovi.Shared.Models;
using BillsNamespace;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sql;

namespace ERacuniNovi.Server.Services
{
    public class BillsService : BillProtoServis.BillProtoServisBase
    {
        private readonly ILogger<BillsService> _logger;
        private readonly BillDBContext _context;
        private readonly ConvertGRPC _convert;

        public BillsService(ILogger<BillsService> log, BillDBContext context, ConvertGRPC convert)
        {
            _logger = log;
            _context = context;
            _convert = convert;
        }

       
        #region Banka Taskovi

        public override async Task<BillPostResponse> ChangeBillBanka(BankaBillMsg request, ServerCallContext context)
        {
            var comparativeBill = _context.Bills.Find(request.ID); //nasao ga u bazi

            _logger.LogInformation($"Nadjen je racun sa tim barcodom u bazi i barcode je {comparativeBill.ID}");

            if (comparativeBill is not null)
            {
                _logger.LogInformation("usao sam u if comparativeBill is not null");
                _logger.LogInformation("ovaj Bill sto sam poslao sam smestio u comparativeBill");
                try
                {
                    _logger.LogInformation("Usao sam u try da sacuvam to u bazu");

                    comparativeBill.Naziv = _convert.ConvertBanka(request).Naziv;
                    comparativeBill.AdresaMusterije = _convert.ConvertBanka(request).AdresaMusterije;
                    comparativeBill.NabavnaCena = _convert.ConvertBanka(request).NabavnaCena;
                    comparativeBill.ProdajnaCena = _convert.ConvertBanka(request).ProdajnaCena;
                    comparativeBill.PresekStanja = _convert.ConvertBanka(request).PresekStanja;
                    comparativeBill.DatumSlanja = _convert.ConvertBanka(request).DatumSlanja;
                    comparativeBill.DatumPrimanja = _convert.ConvertBanka(request).DatumPrimanja;
                    await _context.SaveChangesAsync();

                    return new BillPostResponse { BillResponse = _convert.ConvertCeoRacun(comparativeBill), IsUpisan = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError("Nije nasao racun u bazi da ga izmeni");
                    return new BillPostResponse { IsUpisan = false };
                }
            }
            else
            {
                _logger.LogInformation("comparativeBill je null, nema takvog u bazi, ovo ne treba da se dogadja");
                return new BillPostResponse { IsUpisan = false };
            }
        }

        public override async Task<BillPostResponse> AddBillBanka(BankaBillMsg request, ServerCallContext context)
        {
            _logger.LogInformation("Usao u dodavanje racuna koji je poslat bankom");
            await _context.Bills.ToListAsync();
            Bill racunBanka = _convert.ConvertBanka(request);

            racunBanka.WayOfSellingEnum = Bill.WayOfSelling.Banka;
            racunBanka.Barcode ??= string.Empty;
            try
            {
                _context.Bills.Add(racunBanka);
                await _context.SaveChangesAsync();
                return new BillPostResponse { IsUpisan = true, BillResponse = _convert.ConvertCeoRacun(racunBanka) };
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Nije upisao!");
                _logger.LogError(ex.Message);
                return new BillPostResponse { IsUpisan = false };

            }
        }

        public override async Task GetAllBankaBills(EmptyMsg request, IServerStreamWriter<BillMsg> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Usli smo u GetAllBankaBills");
            var racuniBanka = _context.Bills.Where(a => a.WayOfSellingEnum == Bill.WayOfSelling.Banka).ToList();
            try
            {
                foreach (var item in racuniBanka)
                {
                    _logger.LogInformation("poceo stream");
                    await responseStream.WriteAsync(_convert.ConvertCeoRacun(item));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        #endregion

        
      
    }
}
