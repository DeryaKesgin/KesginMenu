import { useEffect, useRef, useState } from "react";
import { Link } from "react-router";
import QRCode from "react-qr-code";
import api from "../api/api";
import "./QrPage.css";

function QrPage() {
  const user = JSON.parse(localStorage.getItem("user") ?? "{}");

  const qrContainerRef = useRef(null);

  const [business, setBusiness] = useState(null);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadBusiness = async () => {
      if (!user.businessId) {
        setMessage("İşletme bilgisi bulunamadı.");
        setLoading(false);
        return;
      }

      try {
        const response = await api.get(
          `/Businesses/${user.businessId}`
        );

        setBusiness(response.data);
      } catch (error) {
        setMessage(
          error.response?.data?.message ??
            "İşletme bilgisi yüklenemedi."
        );
      } finally {
        setLoading(false);
      }
    };

    loadBusiness();
  }, [user.businessId]);

  const menuUrl = business?.slug
  ? `https://kesgin-menu.vercel.app/menu/${business.slug}`
  : "";

  const copyMenuUrl = async () => {
    try {
      await navigator.clipboard.writeText(menuUrl);
      setMessage("Menü bağlantısı kopyalandı.");
    } catch {
      setMessage("Bağlantı kopyalanamadı.");
    }
  };

  const downloadQrCode = () => {
    const svg = qrContainerRef.current?.querySelector("svg");

    if (!svg) {
      setMessage("QR kod indirilemedi.");
      return;
    }

    const svgClone = svg.cloneNode(true);

    svgClone.setAttribute("xmlns", "http://www.w3.org/2000/svg");

    const svgData = new XMLSerializer().serializeToString(
      svgClone
    );

    const svgBlob = new Blob([svgData], {
      type: "image/svg+xml;charset=utf-8",
    });

    const svgUrl = URL.createObjectURL(svgBlob);
    const image = new Image();

    image.onload = () => {
      const padding = 60;
      const qrSize = 800;

      const canvas = document.createElement("canvas");

      canvas.width = qrSize + padding * 2;
      canvas.height = qrSize + padding * 2;

      const context = canvas.getContext("2d");

      if (!context) {
        URL.revokeObjectURL(svgUrl);
        setMessage("QR kod indirilemedi.");
        return;
      }

      context.fillStyle = "#ffffff";
      context.fillRect(0, 0, canvas.width, canvas.height);

      context.drawImage(
        image,
        padding,
        padding,
        qrSize,
        qrSize
      );

      URL.revokeObjectURL(svgUrl);

      const downloadLink = document.createElement("a");

      downloadLink.download = `${
        business.slug ?? "menu"
      }-qr-kod.png`;

      downloadLink.href = canvas.toDataURL("image/png");
      downloadLink.click();

      setMessage("QR kod PNG olarak indirildi.");
    };

    image.onerror = () => {
      URL.revokeObjectURL(svgUrl);
      setMessage("QR kod indirilemedi.");
    };

    image.src = svgUrl;
  };

  const printQrCode = () => {
    const svg = qrContainerRef.current?.querySelector("svg");

    if (!svg || !business) {
      setMessage("QR kod yazdırılamadı.");
      return;
    }

    const printWindow = window.open("", "_blank");

    if (!printWindow) {
      setMessage("Yazdırma penceresi açılamadı.");
      return;
    }

    printWindow.document.write(`
      <!doctype html>
      <html lang="tr">
        <head>
          <meta charset="UTF-8" />
          <title>${business.name} QR Menü</title>

          <style>
            body {
              margin: 0;
              min-height: 100vh;
              display: flex;
              justify-content: center;
              align-items: center;
              font-family: Arial, sans-serif;
              background: #ffffff;
              color: #24140d;
            }

            .print-card {
              width: 420px;
              padding: 40px;
              border: 2px solid #4f2f20;
              border-radius: 24px;
              text-align: center;
            }

            h1 {
              margin: 0 0 10px;
              font-size: 30px;
            }

            p {
              margin: 0 0 28px;
              color: #715849;
            }

            svg {
              width: 300px;
              height: 300px;
            }

            .url {
              margin-top: 24px;
              font-size: 13px;
              word-break: break-all;
            }
          </style>
        </head>

        <body>
          <section class="print-card">
            <h1>${business.name}</h1>

            <p>Menüyü görüntülemek için QR kodu okutun.</p>

            ${svg.outerHTML}

            <div class="url">${menuUrl}</div>
          </section>

          <script>
            window.onload = function () {
              window.print();
            };
          </script>
        </body>
      </html>
    `);

    printWindow.document.close();
  };

  if (loading) {
    return (
      <main className="qr-page">
        <p>QR kod hazırlanıyor...</p>
      </main>
    );
  }

  if (!business) {
    return (
      <main className="qr-page">
        <Link to="/panel" className="qr-back-link">
          ← Panele dön
        </Link>

        <p>{message || "İşletme bilgisi bulunamadı."}</p>
      </main>
    );
  }

  return (
    <main className="qr-page">
      <header className="qr-page-header">
        <div>
          <Link to="/panel" className="qr-back-link">
            ← Panele dön
          </Link>

          <h1>QR Kod</h1>

          <p>
            Müşterileriniz bu kodu okutarak dijital menünüze
            ulaşabilir.
          </p>
        </div>

        <a
          href={menuUrl}
          target="_blank"
          rel="noreferrer"
          className="qr-open-menu-button"
        >
          Menüyü Aç
        </a>
      </header>

      <section className="qr-content">
        <article className="qr-preview-card">
          <span className="qr-brand">KESGİNSOFT</span>

          <h2>{business.name}</h2>

          <p>Menüyü görüntülemek için QR kodu okutun.</p>

          <div
            ref={qrContainerRef}
            className="qr-code-container"
          >
            <QRCode
              value={menuUrl}
              size={240}
              bgColor="#ffffff"
              fgColor="#24140d"
              level="H"
            />
          </div>

          <strong className="qr-scan-text">
            Kameranızı QR koda yöneltin
          </strong>
        </article>

        <article className="qr-information-card">
          <div>
            <span className="qr-information-label">
              Menü bağlantısı
            </span>

            <h2>QR menünüz hazır</h2>

            <p>
              Bu QR kod işletmenizin güncel menüsüne yönlendirir.
              Kategori veya ürünleri değiştirdiğinizde QR kodu
              yeniden oluşturmanız gerekmez.
            </p>
          </div>

          <div className="qr-url-box">
            <input value={menuUrl} readOnly />

            <button type="button" onClick={copyMenuUrl}>
              Linki Kopyala
            </button>
          </div>

          <div className="qr-action-buttons">
            <button
              type="button"
              className="qr-primary-button"
              onClick={downloadQrCode}
            >
              PNG Olarak İndir
            </button>

            <button
              type="button"
              className="qr-secondary-button"
              onClick={printQrCode}
            >
              Yazdır
            </button>
          </div>

          {message && (
            <p className="qr-message">{message}</p>
          )}
        </article>
      </section>
    </main>
  );
}

export default QrPage;