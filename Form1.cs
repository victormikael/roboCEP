using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http;
using System.Windows.Forms;

namespace roboCEP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnCEP_Click(object sender, EventArgs e)
        {
            
            string cep = txtCEP.Text;
            
            // Validação do CEP
            foreach(char i in cep.ToCharArray())
            {
                if(i < '0' || i > '9')
                {
                    txtRua.ForeColor = Color.Red;
                    txtRua.Text = "Por favor, insira apenas números!";
                    txtBairro.Clear();
                    txtLocal.Clear();

                    return;
                }
            }
            
            if(cep.Length != 8)
            {
                txtRua.ForeColor = Color.Red;
                txtRua.Text = "Por favor, insira um CEP válido!";
                txtBairro.Clear();
                txtLocal.Clear();

                return;
            }            
            

            using (HttpClient httpClient = new HttpClient())
            {
                // Definir endereço do site base
                httpClient.BaseAddress = new Uri("https://buscacepinter.correios.com.br");

                try
                {
                    // Requisição Get Assincrona
                    var cepGet = await httpClient.GetAsync("/app/endereco/carrega-cep-endereco.php?pagina=/app/endereco/index.php&cepaux=&mensagem_alerta=&endereco=" + cep + "&tipoCEP=ALL");

                    // Resposta da requisição convertida em Json
                    dynamic result = JsonConvert.DeserializeObject(await cepGet.Content.ReadAsStringAsync());

                    if (result.total == 0)
                    {
                        txtRua.ForeColor = Color.Red;
                        txtRua.Text = "Nenhum CEP encontrado!";
                        txtBairro.Clear();
                        txtLocal.Clear();
                    }
                    else
                    {
                        txtRua.ForeColor = Color.Black;
                        txtRua.Text = result.dados[0].logradouroDNEC;
                        txtBairro.Text = result.dados[0].bairro;
                        txtLocal.Text = result.dados[0].localidade;

                    }
                }
                catch
                {
                    txtRua.ForeColor = Color.Red;
                    txtRua.Text = "Problemas com a conexão :(";
                    txtBairro.Clear();
                    txtLocal.Clear();

                    return;
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtCEP.Clear();
            txtRua.Clear();
            txtBairro.Clear();
            txtLocal.Clear();
        }
    }
}
