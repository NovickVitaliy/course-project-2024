DO $$
    DECLARE
        first_names TEXT[] := ARRAY['John', 'Jane', 'Alice', 'Bob', 'Charlie', 'Dave', 'Eve', 'Faythe', 'Grace', 'Heidi'];
        last_names TEXT[] := ARRAY['Doe', 'Smith', 'Johnson', 'Williams', 'Brown', 'Jones', 'Garcia', 'Miller', 'Davis', 'Rodriguez'];
        genders TEXT[] := ARRAY[
            'Агендер', 'Андрогін', 'Бігендер', 'Цисгендер', 'Цисгендерна небінарна особа',
            'Демігендер', 'Джендерквір', 'Гендерфлюїд', 'Гендернеконформна особа',
            'Інтерсексуальна особа', 'Нонбінарна особа', 'Пангендер', 'Полігендер',
            'Трансгендерна жінка', 'Трансгендерний чоловік'
            ];
        sexes TEXT[] := ARRAY ['Чоловік', 'Жінка', 'Інтерсекс'];
        sexual_orientations TEXT[] := ARRAY[
            'Гетеросексуальність', 'Гомосексуальність', 'Бісексуальність', 'Пансексуальність',
            'Асексуальність', 'Демісексуальність', 'Полісексуальність', 'Квір',
            'Грейсексуальність', 'Автосексуальність', 'Сколіосексуальність', 'Андросексуальність',
            'Гінекосексуальність'
            ];
        zodiac_signs TEXT[] := ARRAY['Овен', 'Телець', 'Близнюки', 'Рак', 'Лев', 'Діва', 'Терези', 'Скорпіон', 'Стрілець', 'Козеріг', 'Водолій', 'Риби'];
        i INT;
        random_date DATE;
    BEGIN
        FOR i IN 1..500 LOOP
                random_date := date '2023-01-01' + ((random() * (date '2024-07-01' - date '2023-01-01'))::INT);
                INSERT INTO clients (first_name, last_name, gender, sex, sexual_orientation, registration_number, registered_on, age, height, weight, zodiac_sign, description)
                VALUES (
                           first_names[ceil(random() * array_length(first_names, 1))],
                           last_names[ceil(random() * array_length(last_names, 1))],
                           genders[ceil(random() * array_length(genders, 1))],
                           sexes[ceil(random() * array_length(sexes, 1))],
                           sexual_orientations[ceil(random() * array_length(sexual_orientations, 1))],
                           'REG' || lpad(i::text, 3, '0'),
                           random_date,
                           (random() * 52 + 18)::INT, 
                           round((random() * 50 + 150)::NUMERIC, 1), 
                           round((random() * 50 + 50)::NUMERIC, 2),
                           zodiac_signs[ceil(random() * array_length(zodiac_signs, 1))],
                           'Description ' || i
                       );
            END LOOP;
    END $$;
