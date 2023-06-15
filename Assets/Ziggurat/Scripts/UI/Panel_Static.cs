using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    class Panel_Static : MovingPanel
    {
        [SerializeField]
        private Text _timeToCreate_Text;
        [SerializeField]
        private Text _countLife_Text;
        [SerializeField]
        private Text _countDead_Text;

        [SerializeField]
        private Color _specialRed;
        [SerializeField]
        private Color _specialGreen;
        [SerializeField]
        private Color _specialBlue;

        private Colour _lastColour;

        public static Dictionary<Colour, float> _timeToCreate_from_Colour;
        public static Dictionary<Colour, int> _countLife_from_Colour;
        public static Dictionary<Colour, int> _countDead_from_Colour;
                

        private void Awake()
        {
            RayCast.ShowEvent += (colour) => SelectedGate(colour);

            _timeToCreate_from_Colour = new Dictionary<Colour, float>(); //время до создания юнитов по цветам
            _countLife_from_Colour = new Dictionary<Colour, int>();
            _countDead_from_Colour = new Dictionary<Colour, int>();

            _lastColour = Colour.Red;

            SetStartValue();
        }

        //заполнение словарей
        private void SetStartValue()
        {
            for (int i = 0; i < 3; i++)
            {
                _timeToCreate_from_Colour.Add((Colour)i, 0);
                _countLife_from_Colour.Add((Colour)i, 0);
                _countDead_from_Colour.Add((Colour)i, 0);
            }
        }

        private void FixedUpdate()
        {
            SelectedGate(_lastColour);

            //Таймер до создания следующего юнита
            for (int i = 0; i < 3; i++)
                _timeToCreate_from_Colour[(Colour)i] -= Time.deltaTime;
            
        }

        //Учет живых и умерших юнитов
        public void SetCountToDictionary(bool life, Colour colour)
        {
            if (life)
            {
                _countLife_from_Colour[colour] += 1;                
            }
            if (!life)
            {
                _countLife_from_Colour[colour] -= 1;
                _countDead_from_Colour[colour] += 1;
            }
        }        

        //обновление таймера при создании нового юнита
        public void SetTimeToCreat(Colour colour, float time)
        {
            _timeToCreate_from_Colour[colour] = (int)time;
        }                

        //изменение отображения в панеле при выборе зикурата
        private void SelectedGate(Colour colour)
        {
            _lastColour = colour;
            _timeToCreate_Text.text = _timeToCreate_from_Colour[colour].ToString("0.0");
            _countLife_Text.text = _countLife_from_Colour[colour].ToString();
            _countDead_Text.text = _countDead_from_Colour[colour].ToString();

            switch (colour)
            {
                case Colour.Red:
                    _timeToCreate_Text.color = _specialRed;
                    _countLife_Text.color = _specialRed;
                    _countDead_Text.color = _specialRed;
                    break;
                case Colour.Green:
                    _timeToCreate_Text.color = _specialGreen;
                    _countLife_Text.color = _specialGreen;
                    _countDead_Text.color = _specialGreen;
                    break;
                case Colour.Blue:
                    _timeToCreate_Text.color = _specialBlue;
                    _countLife_Text.color = _specialBlue;
                    _countDead_Text.color = _specialBlue;
                    break;
            }
        }

        //метод для кнопки очистки статистики
        public void _clear_CountDead()
        {
            _countDead_from_Colour[_lastColour] = 0;
        }

    }
}
