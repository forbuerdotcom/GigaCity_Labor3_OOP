using System;
using System.Collections.Generic;
using System.Linq;

namespace GigaCity_Labor3_OOP.Models
{
    public class MapModel
    {
        public int Width { get; } = 100;
        public int Height { get; } = 100;
        public List<CellViewModel> Cells { get; private set; }

        public MapModel()
        {
            Cells = GenerateMap();
        }

        private List<CellViewModel> GenerateMap()
        {
            var cells = new List<CellViewModel>();
            var random = new Random(12345);

            // Создаем временный массив для хранения типов местности
            byte[,] terrainMap = new byte[Width, Height];

            // 1. Сначала заполняем все полянами
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    terrainMap[x, y] = 1; // Meadows

            // 2. Рисуем город в центре (70% карты)
            DrawCity(terrainMap);

            // 3. Определяем зоны для природных объектов (не перекрываются)
            var zones = new[]
            {
                new { CenterX = 20, CenterY = 20, Type = (byte)2, Radius = 22 },  // Лес 1
                new { CenterX = 80, CenterY = 80, Type = (byte)2, Radius = 20 },  // Лес 2  
                new { CenterX = 80, CenterY = 20, Type = (byte)2, Radius = 18 },  // Лес 3
                new { CenterX = 15, CenterY = 85, Type = (byte)3, Radius = 16 },  // Горы 1
                new { CenterX = 85, CenterY = 15, Type = (byte)3, Radius = 14 },  // Горы 2
                new { CenterX = 25, CenterY = 50, Type = (byte)4, Radius = 12 },  // Водоем 1
                new { CenterX = 75, CenterY = 50, Type = (byte)4, Radius = 11 }   // Водоем 2
            };

            // 4. Рисуем зоны в порядке приоритета (чтобы не перекрывались)
            foreach (var zone in zones)
            {
                DrawZone(terrainMap, zone.CenterX, zone.CenterY, zone.Radius, zone.Type, random);
            }

            // 5. Создаем ячейки с ресурсами
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = new CellViewModel
                    {
                        X = x,
                        Y = y,
                        TerrainType = terrainMap[x, y],
                        ResourceType = GetResourceType(terrainMap[x, y], random)
                    };
                    cell.ToolTip = $"[{x},{y}] {GetTerrainName(cell.TerrainType)}\nРесурс: {GetResourceName(cell.ResourceType)}";
                    cells.Add(cell);
                }
            }

            return cells;
        }

        private void DrawCity(byte[,] terrainMap)
        {
            int centerX = Width / 2;
            int centerY = Height / 2;
            int cityRadius = 35; // ~70% карты

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                    // Неровные границы с шумом
                    double noise = Math.Sin(x * 0.1) * Math.Cos(y * 0.08) * 10;

                    if (distance < cityRadius + noise)
                    {
                        terrainMap[x, y] = 5; // City
                    }
                }
            }
        }

        private void DrawZone(byte[,] terrainMap, int centerX, int centerY, int baseRadius, byte terrainType, Random random)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Пропускаем городские зоны
                    if (terrainMap[x, y] == 5) continue;

                    double distance = Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));

                    // Создаем неровные границы с разным шумом
                    double noise1 = Math.Sin(x * 0.15 + y * 0.1) * 4;
                    double noise2 = Math.Cos(x * 0.12 - y * 0.08) * 3;
                    double noise3 = Math.Sin(x * 0.08 + y * 0.12) * 5;

                    double effectiveRadius = baseRadius + noise1 + noise2 + noise3;

                    if (distance < effectiveRadius)
                    {
                        terrainMap[x, y] = terrainType;
                    }
                }
            }
        }

        private byte GetResourceType(byte terrainType, Random random)
        {
            if (terrainType == 5) return 0; // No resources in city

            double chance = random.NextDouble();
            return terrainType switch
            {
                1 => chance < 0.2 ? (byte)3 : chance < 0.4 ? (byte)5 : (byte)0, // Meadows: Gas or Plants
                2 => chance < 0.6 ? (byte)4 : chance < 0.8 ? (byte)5 : (byte)0, // Forest: Trees or Plants
                3 => chance < 0.5 ? (byte)1 : chance < 0.7 ? (byte)2 : (byte)0, // Mountains: Metals or Oil
                4 => chance < 0.4 ? (byte)1 : chance < 0.6 ? (byte)2 : (byte)0, // Water: Metals or Oil
                _ => 0
            };
        }

        private string GetTerrainName(byte terrainType)
        {
            return terrainType switch
            {
                1 => "Поляна",
                2 => "Лес",
                3 => "Горы",
                4 => "Водоем",
                5 => "Город",
                _ => "Неизвестно"
            };
        }

        private string GetResourceName(byte resourceType)
        {
            return resourceType switch
            {
                0 => "Нет ресурсов",
                1 => "Металлы",
                2 => "Нефть",
                3 => "Газ",
                4 => "Деревья",
                5 => "Растения",
                _ => "Неизвестно"
            };
        }
    }
}