using Biblioteca.Domain;
using Biblioteca.Services;
using NUlid;


namespace Biblioteca.API
{
    public class Menu
    {
        private readonly ClientService _clientService = new ClientService();

        public void Iniciar()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sistema de BIBLIOTECA DA MARIA");
                Console.WriteLine("1- Cadastrar Cliente");
                Console.WriteLine("2- Listar Clientes");
                Console.WriteLine("3- Atualizar Clients");
                Console.WriteLine("4- Deletar Clientes");

                Console.WriteLine("0 Sair");
                Console.WriteLine("Escolha: ");

                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        RegisterClient();
                        break;
                    case "2":
                        ListClient();
                        break;
                    case "3":
                        UpdateClient();
                        break;
                    case "4":
                        DeleteClient();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Opção Invalida.");
                        break;
                }
                Console.WriteLine("\n Pressione ENTER para continuar ...");
                Console.ReadLine();
            }
        }
        private void RegisterClient()
        {
            Console.Write("Nome: ");
            var nome = Console.ReadLine();

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Telefone: ");
            var telefone = Console.ReadLine();

            var client = new Client
            {
                Id = Ulid.NewUlid().ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Name = nome ?? "",
                Email = email ?? "",
                Phone = telefone ?? "",
            };

            try
            {
                _clientService.RegisterClient(client);
                Console.WriteLine("Oba! Cliente Cadastrado com sucesso !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:" + ex.ToString());
            }
        }

        private void ListClient()
        {
            Console.WriteLine("Entrou no ListClient");
            var clients = _clientService.ListClient();
            Console.WriteLine($"Quantidade de clientes retornados: {clients.Count}");

            if (clients.Count == 0)
            {
                Console.WriteLine("Nenhum Cliente Cadastrado.");
                return;
            }
            foreach (var client in clients)
            {
                Console.WriteLine($"-{client.Name} ({client.Email})");

            }
        }

        private void UpdateClient()
        {
            Console.Write("Informe o ID do cliente para atualizar");
            var id = Console.ReadLine();

            var clients = _clientService.ListClient();
            var client = clients.Find(c => c.Id == id);

            if (client == null)
            {
                Console.WriteLine("Cliente não encontrado .");
                return;
            }

            Console.Write($"Nome ({client.Name}):");
            var nome = Console.ReadLine();
            Console.Write($"Email ({client.Email}):");
            var email = Console.ReadLine();
            Console.Write($"Telefone ({client.Phone}):");
            var telefone = Console.ReadLine();

            client.Name = string.IsNullOrWhiteSpace(nome) ? client.Name : nome;
            client.Email = string.IsNullOrWhiteSpace(email) ? client.Email : email;
            client.Phone = string.IsNullOrWhiteSpace(telefone) ? client.Phone : telefone;
            client.UpdatedAt = DateTime.Now;

            try
            {
                _clientService.UpdateClient(client);
                Console.WriteLine("Cliente atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar cliente: " + ex.Message);
            }
        }

        private void DeleteClient()
        {
            Console.Write("Informe o ID do cliente para deletar: ");
            var id = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(id))
            {
                Console.WriteLine("ID inválido. Operação cancelada.");
                return;
            }
            try
            {
                _clientService.DeleteClient(id);
                Console.WriteLine("Cliente deletado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao deletar cliente: " + ex.Message);
            }

        }
    }
}
