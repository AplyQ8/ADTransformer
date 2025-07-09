import sys
import pandas as pd
import matplotlib.pyplot as plt


def pareto_front(points):
    points = sorted(points, key=lambda x: (x[0], x[1]))
    front = []
    best_y = float('inf')
    for x, y in points:
        if y < best_y:
            front.append((x, y))
            best_y = y
    return front


def main(csv_path):
    df = pd.read_csv(csv_path)
    points = list(zip(df["attackerCost"], df["defenderCost"]))

    front = pareto_front(points)

    # Визуализация
    plt.figure()
    x, y = zip(*points)
    plt.scatter(x, y, label="All points")

    fx, fy = zip(*front)
    plt.plot(fx, fy, color="red", label="Pareto front", marker="o")

    plt.xlabel("Attacker Cost")
    plt.ylabel("Defender Cost")
    plt.title("Pareto Front")
    plt.legend()
    plt.grid(True)
    plt.tight_layout()
    plt.savefig("pareto_front.png")
    print("Pareto front saved to 'pareto_front.png'")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python pareto.py <path_to_csv>")
    else:
        main(sys.argv[1])
