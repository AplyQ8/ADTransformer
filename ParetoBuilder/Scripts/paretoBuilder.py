import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import sys
import os

def plot_attack_defense_costs(csv_file, extend_horizontally=True, save_path=None):
    # Загрузка данных
    df = pd.read_csv(csv_file)
    df.columns = df.columns.str.strip()

    # Сортировка по attackerCost
    df = df.sort_values(by="attackerCost")
    attack_costs = df["attackerCost"].values
    defense_costs = df["defenderCost"].values

    # Подготовка точек для ломаной линии
    line_points_x = []
    line_points_y = []

    real_points = []

    for i in range(len(attack_costs)):
        x = attack_costs[i]
        y = defense_costs[i]
        real_points.append((x, y))

        if i == 0:
            line_points_x.append(x)
            line_points_y.append(y)
        else:
            # горизонтальный переход
            line_points_x.append(x)
            line_points_y.append(line_points_y[-1])
            # вертикальный переход
            line_points_x.append(x)
            line_points_y.append(y)

    # Добавляем стрелку
    last_x = line_points_x[-1]
    last_y = line_points_y[-1]
    if extend_horizontally:
        line_points_x.append(last_x + 1)
        line_points_y.append(last_y)
    else:
        line_points_x.append(last_x)
        line_points_y.append(last_y + 1)

    # Построение графика
    plt.figure(figsize=(8, 6))

    # Красная ломаная линия
    plt.plot(line_points_x, line_points_y, linestyle="solid", color="red", linewidth=2)

    # Определим, нужно ли скрыть последнюю точку
    if not extend_horizontally:
        max_x = max(attack_costs)
        filtered_real_points = [(x, y) for (x, y) in real_points if x != max_x]
    else:
        filtered_real_points = real_points

    # Реальные точки — красные
    real_x, real_y = zip(*real_points)
    plt.scatter(real_x, real_y, color="red", zorder=3)

    # Угловые точки — прозрачные серые
    all_points = set(zip(line_points_x, line_points_y))
    real_point_set = set(real_points)
    corner_points = all_points - real_point_set
    if corner_points:
        cx, cy = zip(*corner_points)
        plt.scatter(cx, cy, color="gray", alpha=0, zorder=2)

    # Отрисовка стрелки
    if extend_horizontally:
        plt.annotate('', xy=(last_x + 1, last_y), xytext=(last_x, last_y),
                     arrowprops=dict(arrowstyle="->", color="red", lw=2))
    else:
        plt.annotate('', xy=(last_x, last_y + 1), xytext=(last_x, last_y),
                     arrowprops=dict(arrowstyle="->", color="red", lw=2))

    # Оформление
    plt.xlabel("Attack Cost")
    plt.ylabel("Defense Cost")
    plt.title("Attack vs Defense Cost (Step-wise Line)")
    plt.grid(True)

    # Сохранение
    if save_path:
        plt.savefig(save_path)
    else:
        default_output_path = os.path.join(os.path.dirname(csv_file), "pareto_front.png")
        plt.savefig(default_output_path)

    plt.close()

# Точка входа
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python plot_attack_defense.py path_to_csv_file [extend_horizontally=True|False] [save_path]")
        sys.exit(1)

    csv_file_path = sys.argv[1]

    extend_horizontally = True
    save_path = None

    if len(sys.argv) >= 3:
        extend_horizontally_str = sys.argv[2].lower()
        if extend_horizontally_str == 'true':
            extend_horizontally = True
        elif extend_horizontally_str == 'false':
            extend_horizontally = False
        else:
            print("Second argument must be True or False")
            sys.exit(1)

    if len(sys.argv) >= 4:
        save_path = sys.argv[3]

    plot_attack_defense_costs(csv_file_path, extend_horizontally, save_path)
