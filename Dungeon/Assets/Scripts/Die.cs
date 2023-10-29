namespace DungeonGame {

public class Die {

	public int d(int max) {

		System.Random die = new System.Random();

		return die.Next(1, max + 1);

	}
}
}
