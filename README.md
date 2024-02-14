# PolarCodingStand (Стенд полярного кодирования)

Стенд полярного кодирования представляет собой графический программный продукт, реализующий функционал моделирования работы зашумлённого канала связи с изменяемыми входными параметрами, в основе которого заложено кодирование передаваемой информации с применением принципа поляризации каналов.

Работу стенда можно описать следующим образом:

1) первым этапом в работе программы идёт задание пользователем параметров моделирования работы канала связи. При подтверждении параметров происходит проверка на корректность введённых значений:
– если они не совпадают с заданными требованиями, программа просит пользователя ввести параметры заново;
– при корректности введённых значений программа переходит к следующему условию;

2) далее пользователь выбирает один из режимов работы программы:
– автоматический режим;
– режим ручного ввода сообщения.
В режиме ручного ввода сообщения пользователь вводит нужное ему информационное слово, далее при подтверждении программа проверяет введённое сообщение на корректность в соответствии с заданным условием:
– если слово не подходит под установленное требование, программа просит пользователя ввести слово заново;
– если подходит, то программа переходит к следующему этапу – последовательное преобразование сообщения по шагам (кодирование, модуляция, демодуляция, декодирование).
Далее на экране наглядно и пошагово показываются все этапы преобразования введённого слова. На этом работа режима ручного ввода сообщения заканчивается;

3) в случае выбора пользователем автоматического режима работы программа моделирует работу канала связи в соответствии с заданными параметрами. Происходит последовательное преобразование сообщения по шагам с добавлением аддитивного белого гауссовского шума (кодирование, модуляция, воздействие шума, демодуляция, декодирование). Помимо этого, программа производит вычисление ошибок, допущенных на этапах демодуляции и декодирования. Эта последовательность операций повторяется в соответствии с заданным условием количества тестов в одной точке (внутренний цикл). Также этот цикл повторяется в соответствии с заданным условием количества запусков модели (внешний цикл). Помимо этого, на экране показываются все этапы преобразования одного из слов моделируемого потока (выполняется единожды). По окончанию моделирования работы канала связи (по прохождению всех циклов запусков модели) программа отображает области Вороного, использованные при демодуляции, а также отображает график отношения вероятности битовой ошибки к отношению сигнал/шум, основываясь на данных по вычислению ошибок, допущенных на этапах демодуляции и декодирования. По окончанию работы автоматического режима пользователь при необходимости может запустить моделирование заново. На этом работа автоматического режима заканчивается;

4) по окончанию работы обоих режимов пользователь имеет возможность вернуться к заданию параметров моделирования и установить другие вводные данные. При отсутствии необходимости в задании других параметров моделирования программа завершает работу (переходит в режим ожидания).

Проект является совместной разработкой владельца профиля и автора, указанного на стартовом экране стенда. Программный продукт доступен для использования и дальнейшего расширения функционала заинтересованными лицами.
