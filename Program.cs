using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Services;

class Program
{
    static void Main(string[] args)
    {
        bool rodando = true;

        while (rodando)
        {
            Console.Clear();
            Console.WriteLine("=== MENU PRINCIPAL ===");
            Console.WriteLine("1 - Cadastrar médico");
            Console.WriteLine("2 - Listar médicos");
            Console.WriteLine("3 - Atualizar médico");
            Console.WriteLine("4 - Deletar médico");
            Console.WriteLine("5 - Cadastrar especialidade");
            Console.WriteLine("6 - Listar especialidades");
            Console.WriteLine("7 - Atualizar especialidade");
            Console.WriteLine("8 - Deletar especialidade");
            Console.WriteLine("9 - Cadastrar paciente");
            Console.WriteLine("10 - Listar pacientes");
            Console.WriteLine("11 - Atualizar paciente");
            Console.WriteLine("12 - Deletar paciente");
            Console.WriteLine("13 - Sair");
            Console.Write("Escolha uma opção: ");

            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    MedicoService.CadastrarMedico();
                    break;
                case "2":
                    MedicoService.ListarMedicos();
                    break;
                case "3":
                    MedicoService.AtualizarMedico();
                    break;
                case "4":
                    MedicoService.DeletarMedico();
                    break;
                case "5":
                    EspecialidadeService.CadastrarEspecialidade();
                    break;
                case "6":
                    EspecialidadeService.ListarEspecialidades();
                    break;
                case "7":
                    EspecialidadeService.AtualizarEspecialidade();
                    break;
                case "8":
                    EspecialidadeService.DeletarEspecialidade();
                    break;
                case "9":
                    PacienteService.CadastrarPaciente();
                    break;
                case "10":
                    PacienteService.ListarPacientes();
                    break;
                case "11":
                    PacienteService.AtualizarPaciente();
                    break;
                case "12":
                    PacienteService.DeletarPaciente();
                    break;
                case "13":
                    rodando = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida! Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
