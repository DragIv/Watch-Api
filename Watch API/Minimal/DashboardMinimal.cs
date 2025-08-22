namespace Watch_API.Minimal;

public static class DashboardMinimal
{
    public static IEndpointConventionBuilder MapDashboard(this IEndpointRouteBuilder app)
    {
        const string html = """
                            <!doctype html>
                            <html>
                            <head>
                            <meta charset="utf-8" />
                            <meta name="viewport" content="width=device-width, initial-scale=1" />
                            <title>CryptoWatch — мини-дашборд</title>
                            <style>
                            body{font-family:system-ui,Segoe UI,Roboto,Arial,sans-serif;margin:24px;background:#0b0f14;color:#dbe1e8}
                            .card{background:#121823;border:1px solid #1f2a3a;padding:16px;border-radius:16px;box-shadow:0 8px 24px #00000040;max-width:900px}
                            h1{margin:0 0 12px;font-size:24px}
                            table{width:100%;border-collapse:collapse}
                            th,td{padding:10px;border-bottom:1px solid #1f2a3a;text-align:left}
                            .muted{opacity:.7}
                            .row{display:flex;gap:16px;flex-wrap:wrap}
                            .pill{display:inline-block;padding:4px 8px;border-radius:999px;background:#1a2332;margin-left:8px;font-size:12px}
                            code{background:#101520;border:1px solid #1f2a3a;padding:2px 6px;border-radius:6px}
                            </style>
                            </head>
                            <body>
                            <div class="card">
                            <h1>CryptoWatch <span class="pill">demo</span></h1>
                            <p class="muted">Рыночные котировки сгенерированы для демо. Рабочие REST-эндпоинты доступны в <code>/swagger</code>.</p>
                            <div class="row">
                                <div style="flex:1">
                                    <h3>Top</h3>
                                    <table id="top">
                                        <thead>
                                            <tr>
                                                <th>Symbol</th>
                                                <th>Price, $</th>
                                                <th>Time (UTC)</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                            </div>
                            <script>
                            console.log('Скрипт загружен.');

                            async function loadTop(){
                                console.log('loadTop() функция запущена');
                                try {
                                    const r = await fetch('/api/market/top');
                                    if (!r.ok) {
                                        const errorBody = await r.text();
                                        throw new Error(`HTTP error! status: ${r.status}, body: ${errorBody}`);
                                    }
                                    const data = await r.json();
                                    const tbody = document.querySelector('#top tbody');
                                    if (tbody) {
                                        tbody.innerHTML = data.map(x =>
                                            `<tr>
                                                <td>${x.symbol}</td>
                                                <td>$${x.priceUsd.toLocaleString()}</td>
                                                <td>${new Date(x.timeUtc).toLocaleString()}</td>
                                            </tr>`
                                        ).join('');
                                        console.log('Данные для Top таблицы загружены успешно.');
                                    } else {
                                        console.error('Элемент #top tbody не найден!');
                                    }
                                } catch (error) {
                                    console.error('Ошибка при загрузке Top цен:', error);
                                }
                            }

                            document.addEventListener('DOMContentLoaded', () => {
                                console.log('DOM полностью загружен. Запускаем loadTop().');
                                loadTop();
                            });
                            </script>
                            </body>
                            </html>
                            """;

        return app.MapGet("/dashboard", () => Results.Content(html, "text/html; charset=utf-8"));
    }
}
