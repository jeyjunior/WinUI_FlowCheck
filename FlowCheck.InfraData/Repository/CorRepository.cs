using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlowCheck.Domain.Entidades;
using FlowCheck.Domain.Interfaces;
using JJ.Net.Data.Interfaces;

namespace FlowCheck.InfraData.Repository
{
    public class CorRepository : Repository<Cor>, ICorRepository
    {
        private static readonly Random _random = new Random();
        private const int MinRGB = 60;
        private const int MaxRGB = 180;

        public CorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Cor GerarCorAleatoria()
        {
            int r = _random.Next(MinRGB, MaxRGB + 1);
            int g = _random.Next(MinRGB, MaxRGB + 1);
            int b = _random.Next(MinRGB, MaxRGB + 1);

            string hexadecimal = RgbToHex(r, g, b);

            return new Cor
            {
                Nome = "CorGenerica",
                Hexadecimal = hexadecimal,
                RGB = $"{r},{g},{b}"
            };
        }

        private string RgbToHex(int r, int g, int b)
        {
            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);

            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
