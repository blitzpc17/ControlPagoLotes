// Archivo: LOGICA/RutasLogica.cs
using DAO.ADOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace LOGICA
{
    public class RutasLogica
    {
        private readonly VariablesGlobalesRepository varsRepo;
        private readonly JavaScriptSerializer json;

        private const string LABEL = "FltroZonas";

        public RutasLogica()
        {
            varsRepo = new VariablesGlobalesRepository();
            json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
        }

        public List<FiltroZonasEntry> GetAll()
        {
            var raw = varsRepo.GetValorByLabel(LABEL);

            if (string.IsNullOrWhiteSpace(raw))
                return new List<FiltroZonasEntry>();

            raw = raw.Trim();
            if (raw == "[]") return new List<FiltroZonasEntry>();

            try
            {
                var list = json.Deserialize<List<FiltroZonasEntry>>(raw);
                return list ?? new List<FiltroZonasEntry>();
            }
            catch
            {
                // Si está corrupto, no revienta la app
                return new List<FiltroZonasEntry>();
            }
        }

        public List<int> GetZonasForUser(int usuarioId)
        {
            var all = GetAll();
            return all.FirstOrDefault(x => x.UsuarioId == usuarioId)?.ZonasId ?? new List<int>();
        }

        public void SetZonasForUser(int usuarioId, List<int> zonasId)
        {
            zonasId = (zonasId ?? new List<int>())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var all = GetAll();

            var entry = all.FirstOrDefault(x => x.UsuarioId == usuarioId);
            if (entry == null)
            {
                all.Add(new FiltroZonasEntry
                {
                    UsuarioId = usuarioId,
                    ZonasId = zonasId
                });
            }
            else
            {
                entry.ZonasId = zonasId;
            }

            // Limpieza: si un usuario se queda sin zonas, puedes decidir quitarlo del JSON:
            all = all.Where(x => x.ZonasId != null && x.ZonasId.Count > 0).ToList();

            var raw = json.Serialize(all);
            varsRepo.UpsertValor(LABEL, raw);
        }
    }
}
