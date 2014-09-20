using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetriPlanet.Core.Collections;

namespace PetriPlanet.Core.Organisms
{
  public static class Mutations
  {
    private static readonly Instruction[] allInstructions = EnumerableExtensions.GetAllEnumValues<Instruction>();

    public static Instruction[] AppendAndPick(Instruction[] motherInstructions, Instruction[] fatherInstructions)
    {
      return motherInstructions
        .Concat(fatherInstructions)
        .Concat(new[] { Instruction.Pick })
        .ToArray();
    }

    public static Instruction[] CrossoverMerge(Random random, Instruction[] motherInstructions, Instruction[] fatherInstructions, int minGeneSize, int maxGeneSize)
    {
      if (fatherInstructions.Length > motherInstructions.Length)
        throw new ArgumentException("motherInstructions must be of equal or greater complexity to fatherInstructions");

      var childIntructions = new Instruction[motherInstructions.Length + fatherInstructions.Length];

      var semiIndex = 0;
      var parity = false;
      while (semiIndex < fatherInstructions.Length) {
        var geneSize = Math.Min(random.Next(minGeneSize, maxGeneSize), fatherInstructions.Length - semiIndex);

        Array.Copy(motherInstructions, semiIndex, childIntructions, 2 * semiIndex, geneSize);
        Array.Copy(fatherInstructions, semiIndex, childIntructions, 2 * semiIndex + geneSize, geneSize);

        semiIndex += geneSize;
        parity = !parity;
      }
      var index = semiIndex * 2;

      if (childIntructions.Length > index)
        Array.Copy(motherInstructions, semiIndex, childIntructions, index, motherInstructions.Length - semiIndex);

      return childIntructions;
    }

    public static Tuple<Instruction[], Instruction[]> CrossoverSplice(Random random, Instruction[] motherInstructions, Instruction[] fatherInstructions, int minGeneSize, int maxGeneSize)
    {
      if (fatherInstructions.Length > motherInstructions.Length)
        throw new ArgumentException("motherInstructions must be of equal or greater complexity to fatherInstructions");

      var daughterIntructions = new Instruction[motherInstructions.Length];
      var sonIntructions = new Instruction[fatherInstructions.Length];

      var index = 0;
      var parity = false;
      while (index < fatherInstructions.Length) {
        var geneSize = Math.Min(random.Next(minGeneSize, maxGeneSize), fatherInstructions.Length - index);

        if (parity) {
          Array.Copy(motherInstructions, index, sonIntructions, index, geneSize);
          Array.Copy(fatherInstructions, index, daughterIntructions, index, geneSize);
        } else {
          Array.Copy(motherInstructions, index, daughterIntructions, index, geneSize);
          Array.Copy(fatherInstructions, index, sonIntructions, index, geneSize);
        }

        index += geneSize;
        parity = !parity;
      }

      if (daughterIntructions.Length > index)
        Array.Copy(motherInstructions, index, daughterIntructions, index, motherInstructions.Length - index);

      return Tuple.Create(daughterIntructions, sonIntructions);
    }

    public static Instruction[] ChangeInstructions(Random random, Instruction[] startingInstructions, int count)
    {
      var newInstructions = (Instruction[]) startingInstructions.Clone();
      for (var i = 0; i < count; i++) {
        var index = random.Next(newInstructions.Length);
        newInstructions[index] = allInstructions.GetRandomElement(random);
      }
      return newInstructions;
    }
  }
}
