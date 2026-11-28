using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace AgriTrace.Infrastructure.Persistence;

public static class AgriTraceDbSeeder
{
    public static async  Task SeedAsync(AgriTraceDbContext db, CancellationToken ct = default)
    {
        // 3 miền
        if (!await db.Regions.AnyAsync(ct))
        {
            var regions = new[]
            {
                Region.Create(1, "Miền Bắc", "NORTH"),
                Region.Create(2, "Miền Trung - Tây Nguyên", "CENTRAL"),
                Region.Create(3, "Miền Nam", "SOUTH")
            };
            await db.Regions.AddRangeAsync(regions, ct);
            await db.SaveChangesAsync(ct);
        }
        // 63 Tỉnh/Thành phố
        if (!await db.Provinces.AnyAsync(ct))
        {
            var provinces = new[]
            {
                // Miền Bắc (25)
                Province.Create(1, 1, "Hà Nội", "HN"),
                Province.Create(2, 1, "Hải Phòng", "HP"),
                Province.Create(3, 1, "Quảng Ninh", "QNI"),
                Province.Create(4, 1, "Bắc Ninh", "BN"),
                Province.Create(5, 1, "Hải Dương", "HD"),
                Province.Create(6, 1, "Hưng Yên", "HY"),
                Province.Create(7, 1, "Thái Bình", "TB"),
                Province.Create(8, 1, "Hà Nam", "HNA"),
                Province.Create(9, 1, "Nam Định", "ND"),
                Province.Create(10, 1, "Ninh Bình", "NB"),
                Province.Create(11, 1, "Vĩnh Phúc", "VP"),
                Province.Create(12, 1, "Phú Thọ", "PT"),
                Province.Create(13, 1, "Bắc Giang", "BG"),
                Province.Create(14, 1, "Thái Nguyên", "TN"),
                Province.Create(15, 1, "Lạng Sơn", "LS"),
                Province.Create(16, 1, "Cao Bằng", "CB"),
                Province.Create(17, 1, "Bắc Kạn", "BK"),
                Province.Create(18, 1, "Tuyên Quang", "TQ"),
                Province.Create(19, 1, "Hà Giang", "HG"),
                Province.Create(20, 1, "Lào Cai", "LC"),
                Province.Create(21, 1, "Yên Bái", "YB"),
                Province.Create(22, 1, "Hòa Bình", "HB"),
                Province.Create(23, 1, "Sơn La", "SL"),
                Province.Create(24, 1, "Điện Biên", "DB"),
                Province.Create(25, 1, "Lai Châu", "LCH"),
                // Miền Trung - Tây Nguyên (19)
                Province.Create(26, 2, "Thanh Hóa", "TH"),
                Province.Create(27, 2, "Nghệ An", "NA"),
                Province.Create(28, 2, "Hà Tĩnh", "HT"),
                Province.Create(29, 2, "Quảng Bình", "QB"),
                Province.Create(30, 2, "Quảng Trị", "QT"),
                Province.Create(31, 2, "Thừa Thiên Huế", "TTH"),
                Province.Create(32, 2, "Đà Nẵng", "DN"),
                Province.Create(33, 2, "Quảng Nam", "QNA"),
                Province.Create(34, 2, "Quảng Ngãi", "QNG"),
                Province.Create(35, 2, "Bình Định", "BD"),
                Province.Create(36, 2, "Phú Yên", "PY"),
                Province.Create(37, 2, "Khánh Hòa", "KH"),
                Province.Create(38, 2, "Ninh Thuận", "NT"),
                Province.Create(39, 2, "Bình Thuận", "BTH"),
                Province.Create(40, 2, "Kon Tum", "KT"),
                Province.Create(41, 2, "Gia Lai", "GL"),
                Province.Create(42, 2, "Đắk Lắk", "DL"),
                Province.Create(43, 2, "Đắk Nông", "DNO"),
                Province.Create(44, 2, "Lâm Đồng", "LD"),
                 // Miền Nam (19)
                Province.Create(45, 3, "TP. Hồ Chí Minh", "HCM"),
                Province.Create(46, 3, "Bình Dương", "BDU"),
                Province.Create(47, 3, "Đồng Nai", "DNI"),
                Province.Create(48, 3, "Bà Rịa - Vũng Tàu", "BRVT"),
                Province.Create(49, 3, "Bình Phước", "BP"),
                Province.Create(50, 3, "Tây Ninh", "TNI"),
                Province.Create(51, 3, "Long An", "LA"),
                Province.Create(52, 3, "Tiền Giang", "TG"),
                Province.Create(53, 3, "Bến Tre", "BTR"),
                Province.Create(54, 3, "Trà Vinh", "TV"),
                Province.Create(55, 3, "Vĩnh Long", "VL"),
                Province.Create(56, 3, "Đồng Tháp", "DT"),
                Province.Create(57, 3, "An Giang", "AG"),
                Province.Create(58, 3, "Kiên Giang", "KG"),
                Province.Create(59, 3, "Cần Thơ", "CT"),
                Province.Create(60, 3, "Hậu Giang", "HGI"),
                Province.Create(61, 3, "Sóc Trăng", "ST"),
                Province.Create(62, 3, "Bạc Liêu", "BL"),
                Province.Create(63, 3, "Cà Mau", "CM")
            };
            await db.Provinces.AddRangeAsync(provinces, ct);
            await db.SaveChangesAsync(ct);
        }
        //  HTX đại diện (3 miền x 3 tỉnh) + 18 sản phẩm OCOP + Chứng nhận + 2 Lô thu hoạch
        if (!await db.Cooperatives.AnyAsync(ct))
        {
            var coopTN = Cooperative.Create(Guid.NewGuid(), 14, "HTX Chè Tân Cương Xanh", "Xã Tân Cương, TP. Thái Nguyên");
            var coopHG = Cooperative.Create(Guid.NewGuid(), 19, "HTX Mật Ong Bạc Hà Đồng Văn", "Huyện Đồng Văn, Hà Giang");
            var coopSL = Cooperative.Create(Guid.NewGuid(), 23, "HTX Nông sản An toàn Mộc Châu", "Thị trấn Nông trường Mộc Châu, Sơn La");
            var coopTTH = Cooperative.Create(Guid.NewGuid(), 31, "HTX Đặc sản Cố Đô Huế", "TP. Huế, Thừa Thiên Huế");
            var coopDL = Cooperative.Create(Guid.NewGuid(), 42, "HTX Cà Phê Bền Vững Ea Tu", "TP. Buôn Ma Thuột, Đắk Lắk");
            var coopLD = Cooperative.Create(Guid.NewGuid(), 44, "HTX Rau Hoa Hữu Cơ Đà Lạt", "Phường 12, TP. Đà Lạt, Lâm Đồng");
            var coopBTR = Cooperative.Create(Guid.NewGuid(), 53, "HTX Dừa Xiêm Xanh Bến Tre", "Huyện Châu Thành, Bến Tre");
            var coopST = Cooperative.Create(Guid.NewGuid(), 61, "HTX Lúa Gạo Thơm Sóc Trăng", "Huyện Mỹ Xuyên, Sóc Trăng");
            var coopDT = Cooperative.Create(Guid.NewGuid(), 56, "HTX Xoài Cao Lãnh Đồng Tháp", "TP. Cao Lãnh, Đồng Tháp");
            var coops = new[]
            {
                coopTN,
                coopHG,
                coopSL,
                coopTTH,
                coopDL,
                coopLD,
                coopBTR,
                coopST,
                coopDT
            };
            foreach (var c in coops)
            {
                c.Verify();
            }
            await db.Cooperatives.AddRangeAsync(coops, ct);
            await db.SaveChangesAsync(ct);
            var issued = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            var expiry = new DateTime(2028, 1, 15, 0, 0, 0, DateTimeKind.Utc);
            AgriProduct MakeProduct(Cooperative coop, string name, string desc, decimal price, int stock, string nutritionJson, params(CertificationType Type, string Grade, string Issuer)[] certs)
            {
                var p = AgriProduct.Create(coop.Id, coop.ProvinceId, name, desc, price, stock, nutritionJson);
                p.Approve();
                foreach (var(type, grade, issuser) in certs)
                {
                    var cert = ProductCertification.Create(p.Id, type, grade, issuser, issued, expiry);
                    cert.Verify();
                    p.AddCertification(cert);
                }
                return p;
            }
            var products = new[]
            {
                // Miền Bắc (6)
                MakeProduct(coopTN, "Chè Đinh Tân Cương Đặc Biệt 500g", "Chè búp non 1 tôm thu hái thủ công tại vùng chè Tân Cương.", 450_000m, 80,
                    """{"caffeine_mg":32,"tanin_percent":14.5,"moisture_percent":4.2}""",
                    (CertificationType.Ocop, "5 Sao", "Hội đồng OCOP Trung ương"), (CertificationType.VietGap, "VG-TN-2026-01", "Sở NN&PTNT Thái Nguyên")),
                MakeProduct(coopTN, "Trà Móc Câu Thái Nguyên 500g", "Hương cốm non tự nhiên, tiền chát dịu hậu ngọt sâu.", 220_000m, 120,
                    """{"caffeine_mg":28,"tanin_percent":13.0,"moisture_percent":4.8}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Thái Nguyên"), (CertificationType.FoodSafety, "ATTP-TN-09", "Chi cục ATVSTP Thái Nguyên")),
                MakeProduct(coopHG, "Mật Ong Hoa Bạc Hà Nguyên Chất 500ml", "Khai thác trên cao nguyên đá Đồng Văn, vị ngọt thanh thơm mát.", 380_000m, 65,
                    """{"energy_kcal":304,"natural_sugar_g":82,"moisture_percent":17.5}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Hà Giang"), (CertificationType.Organic, "ORG-VN-2026-12", "Tổ chức Chứng nhận Hữu cơ VN")),
                MakeProduct(coopHG, "Chè Shan Tuyết Cổ Thụ Hoàng Su Phì 200g", "Thu hái từ cây chè cổ thụ trên 200 năm tuổi ở độ cao 1.500m.", 520_000m, 40,
                    """{"polyphenol_percent":18.2,"caffeine_mg":35,"moisture_percent":4.5}""",
                    (CertificationType.Ocop, "5 Sao", "Hội đồng OCOP Trung ương"), (CertificationType.Organic, "ORG-HG-08", "VN Organic")),
                MakeProduct(coopSL, "Mận Hậu Mộc Châu Sấy Dẻo 300g", "Chế biến từ mận hậu chín cây không đường tinh luyện.", 135_000m, 150,
                    """{"energy_kcal":240,"fiber_g":7.1,"vitamin_c_mg":12}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Sơn La"), (CertificationType.VietGap, "VG-SL-2026-05", "Sở NN&PTNT Sơn La")),
                MakeProduct(coopSL, "Sữa Chua Nếp Cẩm Mộc Châu (Thùng 12 hộp)", "Lên men tự nhiên từ sữa tươi trang trại Mộc Châu.", 145_000m, 90,
                    """{"energy_kcal":115,"protein_g":3.8,"calcium_mg":120}""",
                    (CertificationType.FoodSafety, "ATTP-SL-22", "Chi cục ATVSTP Sơn La"), (CertificationType.Haccp, "HACCP-SL-03", "Vinacert")),
                // Miền Trung - Tây Nguyên (6)
                MakeProduct(coopTTH, "Mè Xửng Thiên Hương Huế 500g", "Đặc sản tiến vua dẻo thơm vị mè rang và đậu phộng.", 95_000m, 200,
                    """{"energy_kcal":410,"protein_g":9.5,"fat_g":14.2}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Thừa Thiên Huế"), (CertificationType.FoodSafety, "ATTP-TTH-11", "Sở Công Thương Huế")),
                MakeProduct(coopTTH, "Tinh Dầu Tràm Gió Nguyên Chất Huế 100ml", "Chưng cất thủ công bằng nồi đồng từ lá tràm gió tự nhiên.", 185_000m, 110,
                    """{"cineol_percent":62.5,"alpha_terpineol_percent":8.4}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Thừa Thiên Huế")),
                MakeProduct(coopDL, "Cà Phê Robusta Honey Buôn Ma Thuột 500g", "Hạt chín đỏ 98% sơ chế mật ong, rang mộc chuẩn vị Tây Nguyên.", 260_000m, 140,
                    """{"caffeine_percent":2.1,"moisture_percent":3.5,"roast_level":"Medium"}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Đắk Lắk"), (CertificationType.VietGap, "VG-DL-2026-19", "Sở NN&PTNT Đắk Lắk")),
                MakeProduct(coopDL, "Hạt Mắc Ca Đắk Lắk Nứt Vỏ 500g", "Sấy lạnh giữ nguyên vị béo bùi và hàm lượng Omega-3 cao.", 290_000m, 95,
                    """{"energy_kcal":718,"healthy_fat_g":75.8,"protein_g":7.9}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Đắk Lắk"), (CertificationType.FoodSafety, "ATTP-DL-07", "Chi cục ATVSTP Đắk Lắk")),
                MakeProduct(coopLD, "Hồng Treo Gió Đà Lạt Công Nghệ Nhật 500g", "Hồng trứng Đà Lạt treo gió tự nhiên 25 ngày, mật ngọt dẻo.", 320_000m, 75,
                    """{"energy_kcal":274,"fiber_g":6.4,"vitamin_a_iu":1800}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Lâm Đồng"), (CertificationType.GlobalGap, "GGAP-LD-2026", "Control Union")),
                MakeProduct(coopLD, "Trà Atiso Túi Lọc Đà Lạt (Hộp 100 túi)", "Chiết xuất từ hoa và thân rễ Atiso trồng hữu cơ tại Đà Lạt.", 165_000m, 180,
                    """{"cynarin_mg":15,"antioxidant_level":"High","sugar_g":0}""",
                    (CertificationType.Organic, "ORG-LD-2026-04", "VN Organic"), (CertificationType.Haccp, "HACCP-LD-18", "Quacert")),
                // Miền Nam (6)
                MakeProduct(coopBTR, "Dầu Dừa Ép Lạnh Hữu Cơ Bến Tre 250ml", "Ép lạnh từ cơm dừa già Bến Tre, giữ trọn axit lauric tự nhiên.", 175_000m, 130,
                    """{"lauric_acid_percent":48.5,"vitamin_e_mg":0.9,"moisture_percent":0.1}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Bến Tre"), (CertificationType.Organic, "ORG-BTR-2026", "Control Union")),
                MakeProduct(coopBTR, "Kẹo Dừa Sáp Bến Tre Ít Ngọt 400g", "Nấu từ nước cốt dừa sáp nguyên chất, thơm béo mềm tan.", 110_000m, 160,
                    """{"energy_kcal":390,"fat_g":18.5,"carbs_g":54.0}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Bến Tre"), (CertificationType.FoodSafety, "ATTP-BTR-14", "Chi cục ATVSTP Bến Tre")),
                MakeProduct(coopST, "Gạo ST25 Sóc Trăng Chính Hãng 5kg", "Gạo ngon nhất thế giới, hạt dài trắng trong, cơm dẻo thơm lá dứa.", 215_000m, 250,
                    """{"energy_kcal":355,"protein_g":7.4,"amylose_percent":16.5}""",
                    (CertificationType.Ocop, "5 Sao", "Hội đồng OCOP Trung ương"), (CertificationType.VietGap, "VG-ST-2026-01", "Sở NN&PTNT Sóc Trăng")),
                MakeProduct(coopST, "Bánh Pía Sầu Riêng Sóc Trăng 480g", "Nhân đậu xanh sầu riêng tươi kết hợp trứng muối truyền thống.", 125_000m, 140,
                    """{"energy_kcal":425,"protein_g":8.2,"fat_g":19.0}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Sóc Trăng"), (CertificationType.Haccp, "HACCP-ST-09", "Vinacert")),
                MakeProduct(coopDT, "Xoài Cát Chu Cao Lãnh Loại 1 (Thùng 3kg)", "Xoài chín cây vàng ươm, thịt dày mịn, ngọt đậm hương thơm dịu.", 240_000m, 85,
                    """{"energy_kcal":60,"vitamin_c_mg":36.4,"brix_level":16.5}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Đồng Tháp"), (CertificationType.GlobalGap, "GGAP-DT-2026-02", "SGS Vietnam")),
                MakeProduct(coopDT, "Hạt Sen Sấy Giòn Đồng Tháp 250g", "Chế biến từ hạt sen Tháp Mười tuyển chọn, giòn bùi không chiên dầu.", 155_000m, 115,
                    """{"energy_kcal":332,"protein_g":15.4,"fiber_g":5.2}""",
                    (CertificationType.Ocop, "4 Sao", "UBND Tỉnh Đồng Tháp"), (CertificationType.FoodSafety, "ATTP-DT-05", "Chi cục ATVSTP Đồng Tháp"))
            };
            await db.AgriProducts.AddRangeAsync(products, ct);
            await db.SaveChangesAsync(ct);
            // 2 Lô thu hoạch mẫu đã khóa QR
            // Chè Tân Cương
            var batchTea = HarvestBatch.Create(products[0].Id, "LOT-TN-2026-001", 450m);
            batchTea.AddFarmingLog(FarmingLog.Create(batchTea.Id, new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc), "Bón phân hữu cơ", "Bón phân vi sinh đầu vụ xuân", 0, "Phân hữu cơ vi sinh Sông Gianh"));
            batchTea.AddFarmingLog(FarmingLog.Create(batchTea.Id, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc), "Phun chế phẩm sinh học", "Phòng trừ rầy xanh bằng chế phẩm thảo mộc", 14, "Chế phẩm Neem sinh học"));
            batchTea.LockAndGenerateQr(new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc), "SEED_HMAC_SIG_TEA_2026_001");
            // Gạo ST25
            var batchRice = HarvestBatch.Create(products[14].Id, "LOT-ST-2026-001", 1200m);
            batchRice.AddFarmingLog(FarmingLog.Create(batchRice.Id, new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc), "Gieo sạ giống ST25", "Gieo sạ lúa xác nhận vụ Đông Xuân", 0, "Giống ST25 chuẩn"));
            batchRice.AddFarmingLog(FarmingLog.Create(batchRice.Id, new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc), "Rút nước trước thu hoạch", "Rút cạn nước ruộng giúp hạt lúa chắc mẩy", 0));
            batchRice.LockAndGenerateQr(new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc), "SEED_HMAC_SIG_ST25_2026_001");

            await db.HarvestBatches.AddRangeAsync(new[]
            {
                batchTea,
                batchRice
            }, ct);
            await db.SaveChangesAsync(ct);
        }
    }
}