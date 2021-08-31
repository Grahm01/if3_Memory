using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int row = 3;
    public int col = 4;

    public float gapRow = 1.5f;
    public float gapCol = 1.5f;

    [Range(0f,5f)]
    public float timeBeforeReset = 1f;
    private bool resetOnGoing = false;

    public GameObject itemPRefab;

    public ItemBehavior[] items;

    public Material[] materials;
    public Material defaultMaterial;

    public List<int> selected = new List<int>(); //déclare une liste de nom "selected"
    public List<int> matches = new List<int>();

    private Dictionary<int, Material> itemMaterial = new Dictionary<int, Material>();

    // Start is called before the first frame update
    void Start()
    {
        items = new ItemBehavior[row * col];
        int index = 0;
        for(int x=0; x<col; x++)
        {
            for (int z=0; z<row; z++) 
            {
                Vector3 position = new Vector3(x * gapCol, 0, z * gapRow);
                GameObject item = Instantiate(itemPRefab, position, Quaternion.identity);
                item.GetComponent<Renderer>().material = defaultMaterial;

                items[index] = item.GetComponent<ItemBehavior>();

                items[index].id = index; // = index pour lui donner un identifiant unique (le but est d'intervenir sur un certain objet unique)
                //atteindre l'id de items[index] (dans lequel on a stocké le composant itemBehavior)
                items[index].manager = this; // this = instance en cours (semblable au "self" dans python)
                index++;
            }
        }
        GiveMaterials();

    }

    private void GiveMaterials()
    {
        List<int> possibilities = new List<int>();
        for (int i=0; i < row *col; i++)
        {
            possibilities.Add(i);
        }

        for (int i=0; i < materials.Length; i++)
        {
            if (possibilities.Count < 2) break;

            int idPos = Random.Range(0, possibilities.Count);
            int id1 = possibilities[idPos];
            possibilities.RemoveAt(idPos);

            idPos = Random.Range(0, possibilities.Count);
            int id2 = possibilities[idPos];
            possibilities.RemoveAt(idPos); //deux fois parce que deux couleurs!

            itemMaterial.Add(id1, materials[i]);
            itemMaterial.Add(id2, materials[i]);

            //items[id1].GetComponent<Renderer>().material = materials[i];
            //items[id2].GetComponent<Renderer>().material = materials[i];
        }
    }


    private IEnumerator ResetMaterials( int id1, int id2) //IEnumerator - à chaque fois qu'il est appelé, il va recommencer depuis le dernier retour qu'il a fait et pas depuis le début
        //co-routine - IMPORTANT
    {
        resetOnGoing = true;
        yield return new WaitForSeconds(timeBeforeReset);
        ResetMaterial(id1);
        ResetMaterial(id2);
        resetOnGoing = false;
    }
    public void RevealMaterials(int id)
    {
        if (resetOnGoing == false && !selected.Contains(id) && !matches.Contains(id))
        {
            selected.Add(id);
            Material material = itemMaterial[id];
            items[id].GetComponent<Renderer>().material = material;
        }

    }

    public void ResetMaterial(int id)
    {//remettre le default material sur l'objet qui a pour id ce qui a été passé en paramètre
        items[id].GetComponent<Renderer>().material = defaultMaterial;


    }
        

    void Update()
    {
        if(selected.Count == 2)
        {
            if(itemMaterial[selected[0]] == itemMaterial[selected[1]])
            {
                Debug.Log("It's a date!");
                matches.Add(selected[0]);
                matches.Add(selected[1]);
            }
            else
            {

                StartCoroutine(ResetMaterials(selected[0], selected[1]));
            }
                selected.Clear();
        }
    }
}
