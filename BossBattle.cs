using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Boss_Battle1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Hello, nihilistic C# World!");
            ReadKey(true);
            Console.Clear();
            Console.Title = "THE HUNT FOR THE MANTICORE";
            WriteLine(" __ THE HUNT FOR THE MANTICORE  __\n\n");

            string presentacion = $"The manticore is attacking the city of Consolas!\n";
            presentacion += $"A cloud tick fog covers the city as its only means of defense\n";
            presentacion += $"the Manticore is not aware of the precise location of the city....\n";
            presentacion += $"but neither is Skorin aware of the location of the Manticore!\n";
            presentacion += $"try to guess the location of the manticore to take it down,\n";
            presentacion += $"before it takes out the city of consolas.\n\n";
            presentacion += $"Be aware, the mere presence of the mighty Manticore ship deals 1 HP of damage\n";
            presentacion += $"to the city of Consolas each turn. Good luck!\n";
            Write(presentacion);
            ReadKey(true);

            int round = 1;

            int[] arreglo_string = {10,15};         // 
            int salud_manticore = arreglo_string[0];  // Salud por defecto del manticore: 10
            int salud_consolas = arreglo_string[1];    // salud por defecto de Consolas: 15.
            int mazorcazo_manticore = 0;
            int mazorcazo_consolas = 0;
            bool descubierto = false;  // El Manticore no sabe donde està consolas.
            bool manticore_down = false;
            bool consolas_destroyed = false;
            

            int[] limite_rango_manticore = { 1, 100 };
            int rango_manticore = numero_random_rango(limite_rango_manticore[0], limite_rango_manticore[1]);
            int rango_consolas = numero_random_rango(1, 100);
            WriteLine($"The city of consolas is located at the \u001b[32mrange {rango_consolas}\u001b[0m");
            WriteLine($"in the perspective from the manticore, but they cant see us!");
            WriteLine("The Manticore is also set in range, but we cant see it either !!\n");
            Write("\n\n");


            // main game loop
            while (round < 15)
            {
                ActualizarBanderas();
                string turno_actual = obtener_string_turno(arreglo_string);
                WriteLine(turno_actual);
                string minenwerfer = kannonenfieber(round);
                WriteLine(minenwerfer);
                string cannon_range = GetCannonRange();
                WriteLine(cannon_range);
                if (manticore_down)
                {
                    ManticoreDown();
                    break;
                }
                string manticore_range = GetManticoreRange();
                WriteLine(manticore_range);
                if (consolas_destroyed)
                {
                    ConsolasDestroyed();
                    break;
                }
                round++;
            }
            //---------------------------------------------------------

            
            
            string obtener_string_turno(int[] parametro_arreglo_string)
                // Forma el string de cada turno,
            {
                ReadKey(true);
                string turno_actual = (new string('_', 70));
                turno_actual += $"\n STATUS: Round {round}, Consolas health: \u001b[33m{parametro_arreglo_string[1]}/15\u001b[0m";
                turno_actual += $", Manticore's health: \u001b[33m{parametro_arreglo_string[0]}/10\u001b[0m\n ";
                return turno_actual;
            }

            //______________________________________
            string kannonenfieber(int artilleriegranate)
            // Dado un int (el turno), regresar un array con el daño que
            // va a hacer el cañon y el texto del cañonazo
            {

                dynamic[] resultado = (artilleriegranate % 3 == 0, artilleriegranate % 5 == 0) switch // Ojo, esto solo funciona en C# 9.0 o superior
                {
                    (true, true) => [10, "migthy fire and electrical blast!"],
                    (true, false) => [3, "fiery blast!"],
                    (false, true) => [5, "shocking rolling thunder!"],
                    (false, false) => [1, "standard 150 mm trench howitzer round!"]
                };

                string resultado_string = "The cannon is expected to";
                resultado_string +=$" deal {resultado[0]} damage via a {resultado[1]}";
                mazorcazo_manticore = resultado[0];  // Para el otro mètodo, en caso que se logre acertar en el Manticore
                return resultado_string;
            }

            //_____________________________________________
            string GetCannonRange()
                // Compara el numero definido por el usuario con el rango
                // del Manticore
            {
                string direct_hit = $"Skorin gets a \u001b[32mDIRECT HIT!\u001b[0m The manticore gets {mazorcazo_manticore} of damage!!";
                string overshot = "Skorin's round \u001b[33mOVERSHOT\u001b[0m the target. No damage was inflicted to the Manticore";
                string fellshort = "Skorin's round \u001b[33mFELL SHORT\u001b[0m to the target. No damage was inflicted to the Manticore";
                string range_return = "";
                Write("Enter the desired cannon range: ");
                int cannon_range = Convert.ToInt32(ReadLine());
                if (cannon_range == rango_manticore)  // Le dimos?
                {
                    range_return = direct_hit;  // Definimos el string
                    salud_manticore -= mazorcazo_manticore;  // y le bajamos la salud al Manticore
                 }
                else if (cannon_range > rango_manticore)  // Muy largo ?
                {
                    range_return = overshot;
                }
                else  // cannon_range < rango_manticore
                {
                    range_return = fellshort;  // muy corto ?
                }
                mazorcazo_manticore = 0; // Reubicamos el daño del manticore en 0 (para que no se sume)
                if (salud_manticore <= 0)
                {
                    manticore_down = true;
                }
                return range_return;
            }

            //_________________________________________________
            string GetManticoreRange()
            {
                // Ensure per-turn manticore damage doesn't accumulate across turns
                string manticore_range = string.Empty;
                mazorcazo_consolas = 0;

                if (descubierto == true)  // El manticore ya descubrio donde està consolas?
                {
                    mazorcazo_consolas = 2;
                    manticore_range = "The Manticore \u001b[31mshots straight\u001b[0m to Consolas!";
                    manticore_range += " and decreases its health by 2 HP.";
                }
                else // El Manticore no ha descubierto a consolas
                {
                    // Asumir este còdigo de bloque solo se ejecuta si el Manticore
                    // no ha descubierto a consolas.
                    manticore_range = "The mere evil presence of the manticore deals";
                    manticore_range += " 1 HP of damage to the city of Consolas!";
                    mazorcazo_consolas = 1;  // Por lo menos le baja 1 de salud en este turno.
                    manticore_range += "\n The Manticore is calculating the range to the city of Consolas...";
                    int ataque_manticore = numero_random_rango(limite_rango_manticore[0], limite_rango_manticore[1]);
                    if (ataque_manticore == rango_consolas)  // Le dio a la ciudad de Consolas?
                    {
                        manticore_range += $"\nThe manticore has \u001b[31mDISCOVERED\u001b[0m our location at range {ataque_manticore}!! ";
                        manticore_range += " Every other attack \u001b[31mwill hit\u001b[0m Consolas directly!";
                        mazorcazo_consolas = 2; // Le baja dos de salud a Consolas en este turno
                        descubierto = true;  // De ahora en adelante el Manticore ya sabe a donde disparar.
                    }
                    else if (ataque_manticore > rango_consolas) // Se paso del rango de Consolas ?
                    {
                        manticore_range += $"\nThe Manticore \u001b[34mOVERSHOT\u001b[0m over consolas with range {ataque_manticore}.";
                        manticore_range += " It will lower its range for the next turn";
                        limite_rango_manticore[1] = (ataque_manticore - 1);
                    }
                    else // Rango muy corto =
                    {
                        manticore_range += $"\nThe Manticore \u001b[34mFELL SHORT\u001b[0m to hit consolas at range {ataque_manticore}.";
                        manticore_range += " It will increase its range for the next turn";
                        limite_rango_manticore[0] = (ataque_manticore + 1); // El -1 y el +1 se asegura que no salgan los mismos numeros
                    }
                }

                salud_consolas -= mazorcazo_consolas;  // Le baja la salud a Consolas.
                mazorcazo_consolas = 0; // Reset so damage doesn't compound across turns
                if (salud_consolas <= 0)
                {
                    consolas_destroyed = true;
                }
                return manticore_range;
            }

            void ManticoreDown()
            {
                WriteLine("Skorin has \u001b[32mDESTROYED\u001b[0m the MANTICORE!");
                WriteLine("Consolas has been \u001b[32mSAVED\u001b[0m !!");
                round = 15;  // solo por si acaso
            }

            void ConsolasDestroyed()
            {
                WriteLine("The City of Consolas has been \u001b[31mDESTROYED!!\u001b[0m");
                WriteLine("Survivors scatter without logic under the influence of the uncoded one!");
                round = 15;  // solo por si acaso
            }

            


            int numero_random_rango(int minimo, int maximo)
                // Obtiene un numero al azar entre los int minimo y màximo.
            {
                Random rango = new Random();
                int rango_random = rango.Next(minimo, maximo);
                return rango_random;
            }

            void ActualizarBanderas()
                // Actualizamos las banderas
            {
                arreglo_string[0] = salud_manticore;
                arreglo_string[1] = salud_consolas;
            }

        }
    }
}
