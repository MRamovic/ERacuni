using BillsNamespace;
using ERacuniNovi.Shared.Models;
using Google.Protobuf.WellKnownTypes;
using ERacuniNovi.Shared.DTO;
using System;

namespace ERacuniNovi.Shared.Konvertor
{
    public class ConvertGRPC
    {
     

        #region Konvertori za Banku

            public BankaBillMsg ConvertBanka(Bill racunBanka) //saljem na server
        {
            BankaBillMsg racunBankaProto = new BankaBillMsg
            {
                Naziv = racunBanka.Naziv, //Naziv artikla sta salje
                AdresaMusterije = racunBanka.AdresaMusterije,
                NabavnaCena = racunBanka.NabavnaCena, //Ono sto mu je uplaceno
                ProdajnaCena = racunBanka.ProdajnaCena, //Ono sto je podigao
                PresekStanja = racunBanka.PresekStanja, //KOliko mu je ostalo
                DatumSlanja = DateTime.SpecifyKind(racunBanka.DatumSlanja, DateTimeKind.Utc).ToTimestamp(),//kad je prodao
                DatumPrimanja = DateTime.SpecifyKind(racunBanka.DatumPrimanja, DateTimeKind.Utc).ToTimestamp(),//kad je podigao novac
                ID = racunBanka.ID
                

        };
            return racunBankaProto;
        }


            public Bill ConvertBanka(BankaBillMsg racunBankaProto) //saljem klijentu
        {
            Bill racunBanka = new Bill
            {
                Naziv = racunBankaProto.Naziv, //Naziv artikla sta salje
                AdresaMusterije = racunBankaProto.AdresaMusterije,
                NabavnaCena = racunBankaProto.NabavnaCena, //Ono sto mu je uplaceno
                ProdajnaCena = racunBankaProto.ProdajnaCena, //Ono sto je podigao
                PresekStanja = racunBankaProto.PresekStanja, //KOliko mu je ostalo
                DatumSlanja = racunBankaProto.DatumSlanja.ToDateTime(),//kad je prodao
                DatumPrimanja = racunBankaProto.DatumPrimanja.ToDateTime(),//kad je podigao novac
                ID = racunBankaProto.ID
            };
            return racunBanka;
        }



        #endregion Konvertori za Banku


    }
}