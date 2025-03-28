using UnityEngine;
using System.Collections;
using Mekaiju.Entity;

namespace Mekaiju
{
    public class RendererMaterialEffect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] [Tooltip("Référence vers le Material à modifier.")]
        private Material _universalRendererMaterial;
        private MechaInstance _mechaInstance;

        [Header("Parameters")]
        [SerializeField] [Tooltip("Seuil en dessous duquel le material affiche un avertissement (ex : 30% de vie).")]
        private float _lowHealthThreshold = 0.3f;
        [SerializeField] [Tooltip("Durée de l'effet de hit.")]
        private float _hitEffectDuration = 0.1f;
        private bool _isFlashing = false;

        [Header("Colors")]
        [SerializeField] [Tooltip("Couleur d'avertissement pour la vie faible.")]
        private Color _lowHealthColor = Color.red;
        [SerializeField] [Tooltip("Couleur d'avertissement pour la vie faible + effet de hit.")]
        private Color _lowHealthPlusHitColor;
        [SerializeField] [Tooltip("Couleur de l'effet de hit.")]
        private Color _hitColor = Color.red;
        [SerializeField] [Tooltip("Couleur par defaut.")]
        private Color _normalColor = Color.black;


        private void Awake()
        {
            _mechaInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<MechaInstance>();
            if (_mechaInstance == null)
            {
                Debug.LogError("MechaInstance introuvable sur ce GameObject !");
            }
            else
            {
                _mechaInstance.onAfterTakeDamage.AddListener(OnAfterTakeDamageHandler);
            }
        }

        private void Update()
        {
            if (!_isFlashing && _mechaInstance != null)
            {
                float t_healthPercent = _mechaInstance.health / _mechaInstance.baseHealth;
                UpdateMaterial(t_healthPercent);
            }
        }

        private void UpdateMaterial(float p_healthPercent)
        {
            // Exemple : si la vie est faible, on augmente l'intensité de l'émission vers le rouge.
            if (p_healthPercent < _lowHealthThreshold)
            {
                _universalRendererMaterial.SetColor("_Color", _lowHealthColor);
                _universalRendererMaterial.SetFloat("_Power", 10f);
            }
            else
            {
                // Remise à zéro si la vie est suffisante.
                _universalRendererMaterial.SetColor("_Color", _normalColor);
                if(_universalRendererMaterial.GetFloat("_Power") == 10f) { _universalRendererMaterial.SetFloat("_Power", 13f); }
            }
        }

        private void OnAfterTakeDamageHandler(IDamageable p_from, float p_damage, DamageKind p_kind)
        {
            if (!_isFlashing) 
            {
                StartCoroutine(DoHitEffect());
            }
        }

        private IEnumerator DoHitEffect()
        {
            if (_universalRendererMaterial == null)
                yield break;

            _isFlashing = true;

            Color t_usedColor = (_mechaInstance.health / _mechaInstance.baseHealth) < _lowHealthThreshold ? _lowHealthPlusHitColor : _hitColor;

            _universalRendererMaterial.SetFloat("_Power", 11f);

            float t_elapsed = 0f;
            while (t_elapsed < _hitEffectDuration)
            {
                float t_temp = Mathf.PingPong(t_elapsed * 2, 1f);
                Color t_currentColor = Color.Lerp(_normalColor, t_usedColor, t_temp);
                _universalRendererMaterial.SetColor("_Color", t_usedColor);
                t_elapsed += Time.deltaTime;
                yield return null;
            }
            // Rétablissement de la couleur en fonction de la vie
            float t_healthPercent = _mechaInstance.health / _mechaInstance.baseHealth;
            if (t_healthPercent < _lowHealthThreshold)
            {
                float t_intensity = Mathf.Lerp(0f, 1f, 1f - t_healthPercent);
                _universalRendererMaterial.SetColor("_Color", _lowHealthColor * t_intensity);
                _universalRendererMaterial.SetFloat("_Power", 10f);
            }
            else
            {
                _universalRendererMaterial.SetColor("_Color", _normalColor);
                _universalRendererMaterial.SetFloat("_Power", 13f);
            }
            _isFlashing = false;
        }
    }
}
